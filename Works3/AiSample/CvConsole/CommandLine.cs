namespace CvConsole;

// 設定のスイッチ (Microsoft.Extensions.Configuration.CommandLine の対応表) と位置引数の取り出し
internal static class CommandLine
{
    public static readonly Dictionary<string, string> Switches = new(StringComparer.OrdinalIgnoreCase)
    {
        ["--endpoint"] = "AIServiceEndPoint",
        ["--key"] = "AIServiceKey",
        ["--out"] = "OutputDirectory",
        ["--show"] = "ShowResult"
    };

    // スイッチ (--name value / --name=value) 以外の引数
    public static string[] GetPositional(string[] args)
    {
        var list = new List<string>();
        for (var i = 0; i < args.Length; i++)
        {
            var arg = args[i];
            if (arg.StartsWith('-') || arg.StartsWith('/'))
            {
                if (!arg.Contains('=', StringComparison.Ordinal))
                {
                    i++;
                }

                continue;
            }

            list.Add(arg);
        }

        return [.. list];
    }
}
