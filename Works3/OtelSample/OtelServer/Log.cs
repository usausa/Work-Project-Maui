namespace OtelServer;

internal static partial class Log
{
    [LoggerMessage(Level = LogLevel.Information, Message = "Received. signal=[{signal}], service=[{service}], count=[{count}]")]
    public static partial void InfoReceived(this ILogger logger, string signal, string service, int count);

    [LoggerMessage(Level = LogLevel.Warning, Message = "Invalid payload. signal=[{signal}]")]
    public static partial void WarnInvalidPayload(this ILogger logger, string signal, Exception exception);
}
