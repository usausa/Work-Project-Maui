namespace OtelServer;

internal static partial class Log
{
    [LoggerMessage(Level = LogLevel.Information, Message = "Received. signal=[{signal}], service=[{service}], count=[{count}]")]
    public static partial void InfoReceived(this ILogger logger, string signal, string service, int count);

    [LoggerMessage(Level = LogLevel.Warning, Message = "Invalid payload. signal=[{signal}]")]
    public static partial void WarnInvalidPayload(this ILogger logger, string signal, Exception exception);

    [LoggerMessage(Level = LogLevel.Information, Message = "Telemetry retention is disabled.")]
    public static partial void InfoRetentionDisabled(this ILogger logger);

    [LoggerMessage(Level = LogLevel.Information, Message = "Telemetry purged. rows=[{rows}], retentionDays=[{retentionDays}]")]
    public static partial void InfoTelemetryPurged(this ILogger logger, int rows, int retentionDays);
}
