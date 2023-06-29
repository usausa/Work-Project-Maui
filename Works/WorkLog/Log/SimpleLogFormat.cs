namespace WorkLog.Log;

using System.Runtime.CompilerServices;

using Microsoft.Extensions.Logging;

public sealed class SimpleLogFormat : LogFormat
{
    public static SimpleLogFormat Instance { get; } = new();

    public override string Format<TState>(
        LogLevel logLevel,
        DateTime timestamp,
        string categoryName,
        TState state,
        Exception? exception,
        Func<TState, Exception?, string> formatter) =>
        $"{timestamp:yyyy/MM/dd HH:mm:ss.fff} [{LogLevelFormat(logLevel)}] ({categoryName}) - {formatter(state, exception)}";

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static string LogLevelFormat(LogLevel level)
    {
        return level switch
        {
            LogLevel.Trace => "TRACE",
            LogLevel.Debug => "DEBUG",
            LogLevel.Information => "INFO",
            LogLevel.Warning => "WARN",
            LogLevel.Error => "ERROR",
            LogLevel.Critical => "CRITICAL",
            _ => "NONE"
        };
    }
}
