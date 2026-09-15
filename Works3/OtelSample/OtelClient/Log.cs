namespace OtelClient;

// 画面の操作と送信基盤が出すログ。ILogger 経由なので送信中は OpenTelemetry へ流れる
internal static partial class Log
{
    [LoggerMessage(Level = LogLevel.Information, Message = "{message}")]
    public static partial void InfoMessage(this ILogger logger, string message);

    [LoggerMessage(Level = LogLevel.Warning, Message = "{message}")]
    public static partial void WarnMessage(this ILogger logger, string message);

    [LoggerMessage(Level = LogLevel.Error, Message = "{message}")]
    public static partial void ErrorMessage(this ILogger logger, string message, Exception exception);

    [LoggerMessage(Level = LogLevel.Information, Message = "Telemetry started. endpoint=[{endpoint}]")]
    public static partial void InfoTelemetryStarted(this ILogger logger, Uri endpoint);

    [LoggerMessage(Level = LogLevel.Information, Message = "Work completed. server=[{server}]")]
    public static partial void InfoWorkCompleted(this ILogger logger, string server);

    [LoggerMessage(Level = LogLevel.Error, Message = "Work failed.")]
    public static partial void ErrorWorkFailed(this ILogger logger, Exception exception);

    [LoggerMessage(Level = LogLevel.Critical, Message = "Unhandled exception.")]
    public static partial void CriticalUnhandled(this ILogger logger, Exception exception);

    [LoggerMessage(Level = LogLevel.Error, Message = "Unobserved task exception.")]
    public static partial void ErrorUnobservedTask(this ILogger logger, Exception exception);
}
