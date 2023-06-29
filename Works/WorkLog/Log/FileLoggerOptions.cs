namespace WorkLog.Log;

using Microsoft.Extensions.Logging;

public sealed class FileLoggerOptions
{
    public bool ShortCategory { get; set; } = true;

    public LogLevel Threshold { get; set; } = LogLevel.Information;

    public LogFormat? Format { get; set; }

    public string Directory { get; set; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Log");

    public string? Prefix { get; set; }

    public int RetainDays { get; set; } = 30;
}
