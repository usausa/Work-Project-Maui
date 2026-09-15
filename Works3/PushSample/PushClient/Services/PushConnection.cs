namespace PushClient.Services;

using System.Net.Sockets;
using System.Net.WebSockets;

using Microsoft.AspNetCore.SignalR.Client;

using PushShared;

public enum PushStatus
{
    Stopped,
    Connecting,
    Connected,
    Reconnecting
}

public sealed class PushMessageEventArgs : EventArgs
{
    public PushMessage Message { get; }

    public PushMessageEventArgs(PushMessage message)
    {
        Message = message;
    }
}

// SignalR の常時接続。
// - 初回の接続失敗は自前でバックオフ再試行する (SignalR の自動再接続は接続確立後の切断だけが対象)
// - 接続後の切断は SignalR の自動再接続 (無期限) に任せ、それでも Closed になったら初回接続からやり直す
// - ネットワークが復帰したらバックオフの待ちを打ち切って即座に試す
// - 停止は CancellationToken で行い、Stop の完了まで待てる
public sealed class PushConnection : IAsyncDisposable
{
    // 再試行の間隔 (以降は最後の値)
    private static readonly TimeSpan[] RetryDelays =
    [
        TimeSpan.Zero,
        TimeSpan.FromSeconds(2),
        TimeSpan.FromSeconds(5),
        TimeSpan.FromSeconds(10),
        TimeSpan.FromSeconds(30),
        TimeSpan.FromSeconds(60)
    ];

    private readonly ILogger<PushConnection> logger;

    private readonly INotifier notifier;

    private readonly SemaphoreSlim lifecycleLock = new(1, 1);

    private readonly SemaphoreSlim resumeSignal = new(0, 1);

    private readonly Lock resumeLock = new();

    private string? currentAddress;

    private CancellationTokenSource? cancellation;

    private Task? runTask;

    private TaskCompletionSource<Exception?>? closedSource;

    // 状態と受信はバックグラウンドスレッドから通知される
    public event EventHandler? StatusChanged;

    public event EventHandler<PushMessageEventArgs>? MessageReceived;

    public PushStatus Status { get; private set; }

    public string? ConnectionId { get; private set; }

    public string? LastError { get; private set; }

    public PushConnection(ILogger<PushConnection> logger, INotifier notifier)
    {
        this.logger = logger;
        this.notifier = notifier;
        Connectivity.Current.ConnectivityChanged += OnConnectivityChanged;
    }

    public async ValueTask DisposeAsync()
    {
        Connectivity.Current.ConnectivityChanged -= OnConnectivityChanged;
        await StopAsync().ConfigureAwait(false);
        lifecycleLock.Dispose();
        resumeSignal.Dispose();
    }

    // 接続を開始する。同じ接続先で動作中なら何もしない、違う接続先なら繋ぎ直す
    public async Task StartAsync(string address)
    {
        await lifecycleLock.WaitAsync().ConfigureAwait(false);
        try
        {
            if ((runTask is not null) && String.Equals(currentAddress, address, StringComparison.Ordinal))
            {
                return;
            }

            await StopCoreAsync().ConfigureAwait(false);

            currentAddress = address;
            cancellation = new CancellationTokenSource();
            runTask = RunAsync(address, cancellation.Token);
        }
        finally
        {
            lifecycleLock.Release();
        }
    }

    public async Task StopAsync()
    {
        await lifecycleLock.WaitAsync().ConfigureAwait(false);
        try
        {
            await StopCoreAsync().ConfigureAwait(false);
        }
        finally
        {
            lifecycleLock.Release();
        }
    }

    private async Task StopCoreAsync()
    {
        if ((runTask is null) || (cancellation is null))
        {
            return;
        }

        try
        {
            await cancellation.CancelAsync().ConfigureAwait(false);
            await runTask.ConfigureAwait(false);
        }
        finally
        {
            cancellation.Dispose();
            cancellation = null;
            runTask = null;
        }
    }

    private async Task RunAsync(string address, CancellationToken token)
    {
        var hub = new HubConnectionBuilder()
            .WithUrl(new Uri(new Uri(address), PushHubInfo.Path))
            .WithAutomaticReconnect(new EndlessRetryPolicy())
            .Build();
        // サーバの KeepAliveInterval (15 秒) / ClientTimeoutInterval (30 秒) と対にする
        hub.ServerTimeout = TimeSpan.FromSeconds(30);
        hub.KeepAliveInterval = TimeSpan.FromSeconds(15);
        using var subscription = hub.On<PushMessage>(PushHubInfo.ReceiveMethod, OnReceived);
        hub.Reconnecting += OnReconnecting;
        hub.Reconnected += OnReconnected;
        hub.Closed += OnClosed;

        try
        {
            while (true)
            {
                closedSource = new TaskCompletionSource<Exception?>(TaskCreationOptions.RunContinuationsAsynchronously);
                await ConnectWithRetryAsync(hub, token).ConfigureAwait(false);

                // 自動再接続を諦めた / サーバに閉じられた (Closed) まで待ち、初回接続からやり直す
                var error = await closedSource.Task.WaitAsync(token).ConfigureAwait(false);
                logger.WarnConnectionClosed(error);
                ConnectionId = null;
                SetStatus(PushStatus.Connecting, error?.Message);
            }
        }
        catch (OperationCanceledException) when (token.IsCancellationRequested)
        {
            // 停止
        }
        finally
        {
            hub.Reconnecting -= OnReconnecting;
            hub.Reconnected -= OnReconnected;
            hub.Closed -= OnClosed;
            await hub.DisposeAsync().ConfigureAwait(false);
            ConnectionId = null;
            SetStatus(PushStatus.Stopped, null);
            logger.InfoStopped();
        }
    }

    private async Task ConnectWithRetryAsync(HubConnection hub, CancellationToken token)
    {
        var attempt = 0;
        while (true)
        {
            token.ThrowIfCancellationRequested();
            try
            {
                SetStatus(PushStatus.Connecting, LastError);
                await hub.StartAsync(token).ConfigureAwait(false);
                ConnectionId = hub.ConnectionId;
                SetStatus(PushStatus.Connected, null);
                logger.InfoConnected(hub.ConnectionId);
                return;
            }
            catch (OperationCanceledException) when (token.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception ex) when (ex is HttpRequestException or IOException or SocketException or WebSocketException or TimeoutException or OperationCanceledException or InvalidOperationException)
            {
                // サーバ停止 / ネットワーク無し / タイムアウト。バックオフして再試行する
                var delay = RetryDelays[Math.Min(attempt, RetryDelays.Length - 1)];
                attempt++;
                logger.WarnConnectFailed(attempt, delay, ex);
                SetStatus(PushStatus.Connecting, ex.Message);
                await WaitForRetryAsync(delay, token).ConfigureAwait(false);
            }
        }
    }

    // バックオフの待ち。ネットワークの復帰で打ち切る
    private async Task WaitForRetryAsync(TimeSpan delay, CancellationToken token)
    {
        if (delay > TimeSpan.Zero)
        {
            await resumeSignal.WaitAsync(delay, token).ConfigureAwait(false);
        }
    }

    private void OnConnectivityChanged(object? sender, ConnectivityChangedEventArgs e)
    {
        if (e.NetworkAccess != NetworkAccess.Internet)
        {
            return;
        }

        lock (resumeLock)
        {
            if (resumeSignal.CurrentCount == 0)
            {
                resumeSignal.Release();
            }
        }
    }

    private Task OnReconnecting(Exception? exception)
    {
        logger.WarnReconnecting(exception);
        SetStatus(PushStatus.Reconnecting, exception?.Message);
        return Task.CompletedTask;
    }

    private Task OnReconnected(string? connectionId)
    {
        logger.InfoReconnected(connectionId);
        ConnectionId = connectionId;
        SetStatus(PushStatus.Connected, null);
        return Task.CompletedTask;
    }

    private Task OnClosed(Exception? exception)
    {
        closedSource?.TrySetResult(exception);
        return Task.CompletedTask;
    }

    private void OnReceived(PushMessage message)
    {
        logger.InfoReceived(message.Title);
        notifier.Show(message);
        MessageReceived?.Invoke(this, new PushMessageEventArgs(message));
    }

    private void SetStatus(PushStatus status, string? error)
    {
        Status = status;
        LastError = error;
        StatusChanged?.Invoke(this, EventArgs.Empty);
    }

    // 自動再接続を諦めない (間隔は RetryDelays の末尾で頭打ち)
    private sealed class EndlessRetryPolicy : IRetryPolicy
    {
        public TimeSpan? NextRetryDelay(RetryContext retryContext) =>
            RetryDelays[(int)Math.Min(retryContext.PreviousRetryCount, RetryDelays.Length - 1)];
    }
}
