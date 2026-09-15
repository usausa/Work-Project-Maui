using Microsoft.AspNetCore.SignalR;

using PushServer;

using PushShared;

var builder = WebApplication.CreateBuilder(args);

// キープアライブとタイムアウトはクライアント側 (KeepAliveInterval / ServerTimeout) と対にする
builder.Services.AddSignalR(static options =>
{
    options.KeepAliveInterval = TimeSpan.FromSeconds(15);
    options.ClientTimeoutInterval = TimeSpan.FromSeconds(30);
});

var app = builder.Build();

app.MapHub<PushHub>(PushHubInfo.Path);

// 接続中の全クライアントへ配信する
// 例: curl -X POST http://localhost:5000/push -H "Content-Type: application/json" -d "{\"title\":\"件名\",\"body\":\"本文\"}"
app.MapPost("/push", static async (PushRequest request, IHubContext<PushHub> hub, ILogger<Program> logger) =>
{
    if (String.IsNullOrWhiteSpace(request.Title))
    {
        return Results.BadRequest("title is required.");
    }

    var message = new PushMessage(request.Title, request.Body ?? string.Empty, DateTimeOffset.Now);
    await hub.Clients.All.SendAsync(PushHubInfo.ReceiveMethod, message).ConfigureAwait(false);
    logger.InfoPushSent(message.Title, PushHub.ConnectionCount);
    return Results.Ok(new PushResponse(message.SentAt, PushHub.ConnectionCount));
});

app.MapGet("/", static () => $"PushServer connections={PushHub.ConnectionCount}");

app.Run();
