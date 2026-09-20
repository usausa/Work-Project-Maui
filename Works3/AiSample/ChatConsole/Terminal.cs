namespace ChatConsole;

// コンソールの入出力
internal static class Terminal
{
    private static int rewriteLength;

    public static void Write(string text) => Console.Write(text);

    public static void WriteLine(string line = "") => Console.WriteLine(line);

    // 同じ行を書き直す (録音中の経過表示)。前回より短い分は空白で消す
    public static void Rewrite(string line)
    {
        Console.Write('\r');
        Console.Write(line);
        if (line.Length < rewriteLength)
        {
            Console.Write(new string(' ', rewriteLength - line.Length));
            Console.Write('\r');
            Console.Write(line);
        }

        rewriteLength = line.Length;
    }

    public static void EndRewrite()
    {
        rewriteLength = 0;
        Console.WriteLine();
    }

    // 入力の前後の空白を除く。EOF は null
    public static string? Prompt(string label)
    {
        Console.Write(label);
        return Console.ReadLine()?.Trim();
    }

    public static bool IsEnterPressed()
    {
        if (Console.IsInputRedirected)
        {
            return false;
        }

        while (Console.KeyAvailable)
        {
            if (Console.ReadKey(true).Key == ConsoleKey.Enter)
            {
                return true;
            }
        }

        return false;
    }
}
