namespace Template.MobileApp.Helpers;

using System.Net.WebSockets;
using System.Reactive;

using Microsoft.AspNetCore.Http.Connections.Client;
using Microsoft.AspNetCore.SignalR.Client;

public enum HubStatusKind
{
    Connecting,
    Connected,
    Reconnecting
}

// 接続の状態。Error は直近の接続失敗 / 切断の理由
public sealed record HubStatus(HubStatusKind Kind, string? ConnectionId, Exception? Error);

// SignalR の HubConnection を Rx で扱う汎用の接続維持 (ハブ固有のパス / メッセージ名 / 型は持たず、利用側はハブ固有の送受信だけを書く)
// - Connect() は購読で接続を始め、購読が破棄されるまで接続し直し続ける
//   初回接続の失敗はバックオフして再試行し (resume で待ちを打ち切る)、接続後の切断は HubConnection の自動再接続 (間隔は同じバックオフ) に任せ、
//   Closed (自動再接続を諦めた / サーバーに閉じられた) になったら初回接続からやり直す
// - Connect() は後勝ち。新しく購読すると前の接続は止め、前の購読者は OnCompleted で終える (同時に 2 本の接続は作らない)
// - On() は受信メッセージの IObservable、InvokeAsync() / IsConnected は現在の接続を使う (接続を作り直しても購読は続く)
// - Dispose() は動作中の接続を止める。以降の Connect() / On() / InvokeAsync() は ObjectDisposedException
public sealed class ReactiveHubConnection : IDisposable
{
    // 再試行の間隔 (以降は末尾の値)
    private static readonly TimeSpan[] DefaultRetryDelays =
    [
        TimeSpan.Zero,
        TimeSpan.FromSeconds(2),
        TimeSpan.FromSeconds(5),
        TimeSpan.FromSeconds(10),
        TimeSpan.FromSeconds(30),
        TimeSpan.FromSeconds(60)
    ];

    private readonly IReadOnlyList<TimeSpan> retryDelays;

    private readonly TimeSpan? serverTimeout;

    private readonly TimeSpan? keepAliveInterval;

    // 自動再接続の間隔 (retryDelays のバックオフ)
    private readonly IRetryPolicy reconnectPolicy;

    private readonly Lock sync = new();

    // 現在の接続 (Connect() の購読中だけ存在する)
    private readonly BehaviorSubject<HubConnection?> connections = new(null);

    // 動作中のセッション (後勝ち。新しい購読で入れ替える)
    private Session? current;

    // これまでのセッションの停止 (次のセッションはこれを待ってから接続する)
    private Task stopping = Task.CompletedTask;

    private bool disposed;

    public bool IsConnected => connections.TryGetValue(out var connection) && (connection?.State == HubConnectionState.Connected);

    public ReactiveHubConnection(TimeSpan? serverTimeout = null, TimeSpan? keepAliveInterval = null, IReadOnlyList<TimeSpan>? retryDelays = null)
    {
        this.serverTimeout = serverTimeout;
        this.keepAliveInterval = keepAliveInterval;
        this.retryDelays = retryDelays ?? DefaultRetryDelays;
        ArgumentOutOfRangeException.ThrowIfZero(this.retryDelays.Count);
        reconnectPolicy = new BackoffRetryPolicy(this.retryDelays);
    }

    // 動作中のセッションを止めてから (購読者は OnCompleted で終わる) 内部の Subject を破棄する
    public void Dispose()
    {
        Session? session;
        lock (sync)
        {
            if (disposed)
            {
                return;
            }

            disposed = true;
            session = current;
            current = null;
        }

        session?.Dispose();
        connections.Dispose();
    }

    // 接続を維持する。購読で接続を始め、購読が破棄されるまで接続し直し続ける (新しい購読で前の購読は終わる)
    public IObservable<HubStatus> Connect(Uri url, Action<HttpConnectionOptions>? configure = null, IObservable<Unit>? resume = null)
    {
        ObjectDisposedException.ThrowIf(disposed, this);

        return Observable.Create<HubStatus>(observer =>
        {
            var session = new Session(CreateConnection(url, configure));
            Session? previous;
            lock (sync)
            {
                if (disposed)
                {
                    session.Dispose();
                }

                ObjectDisposedException.ThrowIf(disposed, this);

                previous = current;
                current = session;
                if (previous is not null)
                {
                    stopping = Task.WhenAll(stopping, previous.Stopped);
                }

                connections.OnNext(session.Connection);
                session.Start(Connect(session.Connection, stopping, resume ?? Observable.Never<Unit>()), observer);
            }

            // 前の購読者を終えて接続を止める (新しい接続は停止を待ってから始まる)
            previous?.Dispose();

            return Disposable.Create(() => Release(session));
        });
    }

    // 受信メッセージ。購読中だけハンドラを登録する
    public IObservable<T> On<T>(string methodName)
    {
        ObjectDisposedException.ThrowIf(disposed, this);

        return connections
            .Select(connection => connection is null
                ? Observable.Empty<T>()
                : Observable.Create<T>(observer => connection.On<T>(methodName, observer.OnNext)))
            .Switch();
    }

    // 接続中なら送る (未接続は送らずに false)
    public ValueTask<bool> InvokeAsync(string methodName, CancellationToken cancellationToken = default) =>
        InvokeAsync(methodName, [], cancellationToken);

    public ValueTask<bool> InvokeAsync(string methodName, object? argument, CancellationToken cancellationToken = default) =>
        InvokeAsync(methodName, [argument], cancellationToken);

    private async ValueTask<bool> InvokeAsync(string methodName, object?[] arguments, CancellationToken cancellationToken)
    {
        ObjectDisposedException.ThrowIf(disposed, this);

        if (!connections.TryGetValue(out var connection) || (connection is null) || (connection.State != HubConnectionState.Connected))
        {
            return false;
        }

        await connection.InvokeCoreAsync(methodName, arguments, cancellationToken).ConfigureAwait(false);
        return true;
    }

    // 購読の破棄。現在のセッションなら止める (置き換え済み / Dispose 済みなら何もしない)
    private void Release(Session session)
    {
        lock (sync)
        {
            if (!ReferenceEquals(current, session))
            {
                return;
            }

            current = null;
            stopping = Task.WhenAll(stopping, session.Stopped);
            connections.OnNext(null);
        }

        session.Dispose();
    }

    private HubConnection CreateConnection(Uri url, Action<HttpConnectionOptions>? configure)
    {
        var builder = new HubConnectionBuilder().WithAutomaticReconnect(reconnectPolicy);
        var connection = (configure is null ? builder.WithUrl(url) : builder.WithUrl(url, configure)).Build();
        if (serverTimeout is { } timeout)
        {
            connection.ServerTimeout = timeout;
        }
        if (keepAliveInterval is { } interval)
        {
            connection.KeepAliveInterval = interval;
        }

        return connection;
    }

    private IObservable<HubStatus> Connect(HubConnection connection, Task ready, IObservable<Unit> resume) =>
        Observable.Create<HubStatus>(observer =>
        {
            // HubConnection のイベント (Func<T, Task>) を Observable にする
            var closed = new Subject<Exception?>();
            var reconnecting = new Subject<Exception?>();
            var reconnected = new Subject<string?>();
            Task OnClosed(Exception? exception)
            {
                closed.OnNext(exception);
                return Task.CompletedTask;
            }

            Task OnReconnecting(Exception? exception)
            {
                reconnecting.OnNext(exception);
                return Task.CompletedTask;
            }

            Task OnReconnected(string? connectionId)
            {
                reconnected.OnNext(connectionId);
                return Task.CompletedTask;
            }

            connection.Closed += OnClosed;
            connection.Reconnecting += OnReconnecting;
            connection.Reconnected += OnReconnected;

            // 1 回の接続試行 (前の接続の停止を待ってから)。失敗は理由を流してから RetryWhen へ
            var attempt = Observable.FromAsync(async cancel =>
                {
                    await ready.WaitAsync(cancel).ConfigureAwait(false);
                    await connection.StartAsync(cancel).ConfigureAwait(false);
                })
                .Select(_ => new HubStatus(HubStatusKind.Connected, connection.ConnectionId, null))
                .Catch(static (Exception ex) => Observable.Return(new HubStatus(HubStatusKind.Connecting, null, ex)).Concat(Observable.Throw<HubStatus>(ex)));

            // 初回接続: バックオフして再試行 (ネットワークの復帰で待ちを打ち切る)
            var connect = attempt.RetryWhen(errors => errors
                .Select(static (_, index) => index)
                .SelectMany(index => Observable.Timer(retryDelays[Math.Min(index, retryDelays.Count - 1)])
                    .Select(static _ => Unit.Default)
                    .Amb(resume.Take(1))));

            // 接続 → Closed → 初回接続からやり直し
            var session = connect
                .Concat(closed.Take(1).Select(static ex => new HubStatus(HubStatusKind.Connecting, null, ex)))
                .Repeat();

            var subscription = Observable.Merge(
                    session,
                    reconnecting.Select(static ex => new HubStatus(HubStatusKind.Reconnecting, null, ex)),
                    reconnected.Select(static id => new HubStatus(HubStatusKind.Connected, id, null)))
                .StartWith(new HubStatus(HubStatusKind.Connecting, null, null))
                .Subscribe(observer);

            return new CompositeDisposable(
                subscription,
                Disposable.Create(() =>
                {
                    connection.Closed -= OnClosed;
                    connection.Reconnecting -= OnReconnecting;
                    connection.Reconnected -= OnReconnected;
                }),
                closed,
                reconnecting,
                reconnected);
        });

    // 1 回の Connect() の購読 = HubConnection の寿命
    private sealed class Session : IDisposable
    {
        // 監視の終了 (TakeUntil で購読者を OnCompleted で終える)
        private readonly Subject<Unit> stop = new();

        private readonly TaskCompletionSource stopped = new(TaskCreationOptions.RunContinuationsAsynchronously);

        private IDisposable? subscription;

        public HubConnection Connection { get; }

        // 接続の停止と破棄の完了
        public Task Stopped => stopped.Task;

        public Session(HubConnection connection)
        {
            Connection = connection;
        }

        public void Start(IObservable<HubStatus> status, IObserver<HubStatus> observer)
        {
            subscription = status.TakeUntil(stop).Subscribe(observer);
        }

        // 監視をやめて購読者を終え、接続を止めて破棄する
        public void Dispose()
        {
            stop.OnNext(Unit.Default);
            stop.Dispose();
            subscription?.Dispose();
            _ = StopAsync();
        }

        private async Task StopAsync()
        {
            try
            {
                await Connection.StopAsync().ConfigureAwait(false);
                await Connection.DisposeAsync().ConfigureAwait(false);
            }
            catch (Exception ex) when (ex is InvalidOperationException or IOException or WebSocketException)
            {
                // 切断中の失敗は無視する
            }
            finally
            {
                stopped.SetResult();
            }
        }
    }

    // 自動再接続のポリシー (諦めない。間隔は delays の末尾で頭打ち)
    private sealed class BackoffRetryPolicy : IRetryPolicy
    {
        private readonly IReadOnlyList<TimeSpan> delays;

        public BackoffRetryPolicy(IReadOnlyList<TimeSpan> delays)
        {
            this.delays = delays;
        }

        public TimeSpan? NextRetryDelay(RetryContext retryContext) =>
            delays[(int)Math.Min(retryContext.PreviousRetryCount, delays.Count - 1)];
    }
}
