namespace PushServer;

internal static partial class Log
{
    [LoggerMessage(Level = LogLevel.Information, Message = "Client connected. id=[{connectionId}], connections=[{count}]")]
    public static partial void InfoConnected(this ILogger logger, string connectionId, int count);

    [LoggerMessage(Level = LogLevel.Information, Message = "Client disconnected. id=[{connectionId}], connections=[{count}]")]
    public static partial void InfoDisconnected(this ILogger logger, string connectionId, int count, Exception? exception);

    [LoggerMessage(Level = LogLevel.Information, Message = "Push sent. title=[{title}], connections=[{count}]")]
    public static partial void InfoPushSent(this ILogger logger, string title, int count);
}
