namespace WorkLog.Log;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

internal sealed class FileLoggerProvider : ILoggerProvider
{
    private readonly FileLoggerOptions options;

    private readonly FileLoggerWriter writer;

    public FileLoggerProvider(IOptions<FileLoggerOptions> options)
    {
        this.options = options.Value;
        writer = new FileLoggerWriter(
            options.Value.Directory ,
            options.Value.Prefix,
            options.Value.RetainDays,
            options.Value.Format ?? SimpleLogFormat.Instance);
    }

    public void Dispose()
    {
        writer.Dispose();
    }

    public ILogger CreateLogger(string categoryName)
    {
        if (options.ShortCategory)
        {
            var index = categoryName.LastIndexOf('.');
            if (index >= 0)
            {
                categoryName = categoryName[(index + 1)..];
            }
        }

        return new FileLogger(categoryName, options.Threshold, writer);
    }
}
