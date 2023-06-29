using Microsoft.Extensions.Logging;

namespace WorkLog.Log;

public static class LoggingBuilderExtensions
{
    public static ILoggingBuilder AddAndroidLogger(this ILoggingBuilder builder)
    {
        return builder.AddAndroidLogger(_ => { });
    }

    public static ILoggingBuilder AddAndroidLogger(this ILoggingBuilder builder, Action<AndroidLoggerOptions> configure)
    {
        builder.Services.AddSingleton<ILoggerProvider, AndroidLoggerProvider>();
        builder.Services.Configure(configure);
        return builder;
    }

    public static ILoggingBuilder AddFileLogger(this ILoggingBuilder builder)
    {
        return builder.AddFileLogger(_ => { });
    }

    public static ILoggingBuilder AddFileLogger(this ILoggingBuilder builder, Action<FileLoggerOptions> configure)
    {
        builder.Services.AddSingleton<ILoggerProvider, FileLoggerProvider>();
        builder.Services.Configure(configure);
        return builder;
    }
}
