namespace Template.MobileApp.Helpers;

using System.Net.WebSockets;
using System.Reactive;

using Microsoft.AspNetCore.SignalR.Client;

public enum HubStatusKind
{
    Connecting,
    Connected,
    Reconnecting
}

// 接続の状態。Error は直近の接続失敗 / 切断の理由
public sealed record HubStatus(HubStatusKind Kind, string? ConnectionId, Exception? Error);

// SignalR の HubConnection を Rx で扱う
// - Connect() は購読で接続を始め、購読の破棄で切断する
// - 初回接続の失敗はバックオフして再試行し (resume で待ちを打ち切る)、接続後の切断は HubConnection の自動再接続に任せ、
//   Closed (自動再接続を諦めた / サーバーに閉じられた) になったら初回接続からやり直す
// - On() は受信メッセージの IObservable (購読中だけハンドラを登録する)
public static class ReactiveSignalR
{
    // 再試行の間隔 (以降は末尾の値)
    public static IReadOnlyList<TimeSpan> DefaultRetryDelays { get; } =
    [
        TimeSpan.Zero,
        TimeSpan.FromSeconds(2),
        TimeSpan.FromSeconds(5),
        TimeSpan.FromSeconds(10),
        TimeSpan.FromSeconds(30),
        TimeSpan.FromSeconds(60)
    ];

    // 自動再接続のポリシー (諦めない。間隔は delays の末尾で頭打ち)
    public static IRetryPolicy CreateRetryPolicy(IReadOnlyList<TimeSpan> delays) => new BackoffRetryPolicy(delays);

    public static IObservable<T> On<T>(this HubConnection connection, string methodName) =>
        Observable.Create<T>(observer => connection.On<T>(methodName, observer.OnNext));

    public static IObservable<HubStatus> Connect(this HubConnection connection, IReadOnlyList<TimeSpan> retryDelays, IObservable<Unit>? resume = null)
    {
        ArgumentOutOfRangeException.ThrowIfZero(retryDelays.Count);

        return Observable.Create<HubStatus>(observer =>
        {
            var resumeSignal = resume ?? Observable.Never<Unit>();

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

            // 1 回の接続試行。失敗は理由を流してから RetryWhen へ
            var attempt = Observable.FromAsync(connection.StartAsync)
                .Select(_ => new HubStatus(HubStatusKind.Connected, connection.ConnectionId, null))
                .Catch(static (Exception ex) => Observable.Return(new HubStatus(HubStatusKind.Connecting, null, ex)).Concat(Observable.Throw<HubStatus>(ex)));

            // 初回接続: バックオフして再試行 (ネットワークの復帰で待ちを打ち切る)
            var connect = attempt.RetryWhen(errors => errors
                .Select(static (_, index) => index)
                .SelectMany(index => Observable.Timer(retryDelays[Math.Min(index, retryDelays.Count - 1)])
                    .Select(static _ => Unit.Default)
                    .Amb(resumeSignal.Take(1))));

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
                    _ = StopAsync(connection);
                }),
                closed,
                reconnecting,
                reconnected);
        });
    }

    private static async Task StopAsync(HubConnection connection)
    {
        try
        {
            await connection.StopAsync().ConfigureAwait(false);
        }
        catch (Exception ex) when (ex is InvalidOperationException or IOException or WebSocketException)
        {
            // 破棄後の停止 / 切断中の失敗は無視する
        }
    }

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
