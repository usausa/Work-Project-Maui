namespace WorkLog.Log;

public sealed class AndroidLoggerOptions
{
    public bool ShortCategory { get; set; } = true;

    public LogFormat? Format { get; set; }
}
