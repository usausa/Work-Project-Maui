namespace WorkLog.Log;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

public sealed class AndroidLoggerProvider : ILoggerProvider
{
    private readonly bool shortCategory;

    private readonly LogFormat? format;

    public AndroidLoggerProvider(IOptions<AndroidLoggerOptions> options)
    {
        shortCategory = options.Value.ShortCategory;
        format = options.Value.Format;
    }

    public void Dispose()
    {
    }

    public ILogger CreateLogger(string categoryName)
    {
        return new AndroidLogger(categoryName, LogLevel.Debug, format ?? MessageLogFormat.Instance);
    }
}
