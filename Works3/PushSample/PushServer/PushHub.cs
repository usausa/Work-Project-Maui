namespace PushServer;

using Microsoft.AspNetCore.SignalR;

// 接続の出入りを数えて記録するだけのハブ。配信は IHubContext から行う
// CA1812: インスタンス化は SignalR が行う
#pragma warning disable CA1812
internal sealed class PushHub : Hub
{
    private static int connectionCount;

    private readonly ILogger<PushHub> logger;

    public static int ConnectionCount => Volatile.Read(ref connectionCount);

    public PushHub(ILogger<PushHub> logger)
    {
        this.logger = logger;
    }

    public override Task OnConnectedAsync()
    {
        var count = Interlocked.Increment(ref connectionCount);
        logger.InfoConnected(Context.ConnectionId, count);
        return base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        var count = Interlocked.Decrement(ref connectionCount);
        logger.InfoDisconnected(Context.ConnectionId, count, exception);
        return base.OnDisconnectedAsync(exception);
    }
}
