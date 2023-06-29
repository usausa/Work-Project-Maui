namespace WorkLog.Log;

using System.Text;

using Microsoft.Extensions.Logging;

internal sealed class FileLoggerWriter : IDisposable
{
    private readonly object sync = new();

    private readonly string directory;

    private readonly string prefix;

    private readonly int retainDays;

    private readonly LogFormat format;

    private StreamWriter? writer;

    private DateTime lastDate = DateTime.MinValue.Date;

    public FileLoggerWriter(string directory, string prefix, int retainDays, LogFormat format)
    {
        this.directory = directory;
        this.prefix = prefix;
        this.retainDays = retainDays;
        this.format = format;

        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory!);
        }
    }

    public void Dispose()
    {
        writer?.Dispose();
    }

    public void Write<TState>(LogLevel logLevel, string categoryName, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        lock (sync)
        {
            var timestamp = DateTime.Now;
            var message = format.Format(logLevel, timestamp, categoryName, state, exception, formatter);

            var date = timestamp.Date;
            if ((lastDate < date) || (writer is null))
            {
                writer?.Dispose();
                writer = CreateWriter(date);

                // TODO delete old

                lastDate = date;
            }

            writer!.WriteLine(message);
            writer.Flush();
        }
    }

    private StreamWriter CreateWriter(DateTime timestamp)
    {
        var builder = new StringBuilder();
        if (!String.IsNullOrEmpty(prefix))
        {
            builder.Append(prefix);
        }

        builder.Append(timestamp.ToString("yyyyMMdd"));
        builder.Append(".log");

        var filename = Path.Combine(directory, builder.ToString());
        var fileStream = File.Open(filename, FileMode.OpenOrCreate, FileAccess.ReadWrite);
        fileStream.Seek(0, SeekOrigin.End);
        return new StreamWriter(fileStream);
    }
}
