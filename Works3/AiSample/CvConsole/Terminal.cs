namespace CvConsole;

// コンソールの入出力
internal static class Terminal
{
    public static void WriteLine(string line = "") => Console.WriteLine(line);

    // 入力の前後の空白と、ファイルのドラッグ & ドロップで付く引用符を除く。EOF は null
    public static string? Prompt(string label)
    {
        Console.Write(label);
        return Console.ReadLine()?.Trim().Trim('"');
    }
}
