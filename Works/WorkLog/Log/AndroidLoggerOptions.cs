﻿using Microsoft.Extensions.Logging;

namespace WorkLog.Log;

public sealed class AndroidLoggerOptions
{
    public bool ShortCategory { get; set; }

    public LogLevel Threshold { get; set; } = LogLevel.Trace;

    public LogFormat? Format { get; set; }
}
