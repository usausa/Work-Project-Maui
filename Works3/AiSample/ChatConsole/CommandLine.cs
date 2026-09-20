namespace ChatConsole;

// 設定のスイッチ (Microsoft.Extensions.Configuration.CommandLine の対応表)
internal static class CommandLine
{
    public static readonly Dictionary<string, string> Switches = new(StringComparer.OrdinalIgnoreCase)
    {
        ["--endpoint"] = "OllamaEndPoint",
        ["--model"] = "OllamaModel",
        ["--speech"] = "Speech"
    };
}
