namespace PushClient;

internal static partial class Log
{
    [LoggerMessage(Level = LogLevel.Information, Message = "Connected. id=[{connectionId}]")]
    public static partial void InfoConnected(this ILogger logger, string? connectionId);

    [LoggerMessage(Level = LogLevel.Warning, Message = "Connect failed. attempt=[{attempt}], retryAfter=[{delay}]")]
    public static partial void WarnConnectFailed(this ILogger logger, int attempt, TimeSpan delay, Exception exception);

    [LoggerMessage(Level = LogLevel.Warning, Message = "Reconnecting.")]
    public static partial void WarnReconnecting(this ILogger logger, Exception? exception);

    [LoggerMessage(Level = LogLevel.Information, Message = "Reconnected. id=[{connectionId}]")]
    public static partial void InfoReconnected(this ILogger logger, string? connectionId);

    [LoggerMessage(Level = LogLevel.Warning, Message = "Connection closed.")]
    public static partial void WarnConnectionClosed(this ILogger logger, Exception? exception);

    [LoggerMessage(Level = LogLevel.Information, Message = "Stopped.")]
    public static partial void InfoStopped(this ILogger logger);

    [LoggerMessage(Level = LogLevel.Information, Message = "Message received. title=[{title}]")]
    public static partial void InfoReceived(this ILogger logger, string title);

    [LoggerMessage(Level = LogLevel.Warning, Message = "Foreground service start is not allowed.")]
    public static partial void WarnForegroundNotAllowed(this ILogger logger, Exception exception);
}
