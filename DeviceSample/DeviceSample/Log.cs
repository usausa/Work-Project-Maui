namespace DeviceSample;

internal static partial class Log
{
    [LoggerMessage(Level = LogLevel.Information, Message = "Application start.")]
    public static partial void InfoApplicationStart(this ILogger logger);

    [LoggerMessage(Level = LogLevel.Information, Message = "Runtime: os=[{osDescription}], framework=[{frameworkDescription}], rid=[{runtimeIdentifier}]")]
    public static partial void InfoApplicationSettingsRuntime(this ILogger logger, string osDescription, string frameworkDescription, string runtimeIdentifier);

    [LoggerMessage(Level = LogLevel.Information, Message = "Environment: version=[{version}], directory=[{directory}]")]
    public static partial void InfoApplicationSettingsEnvironment(this ILogger logger, Version? version, string directory);
}
