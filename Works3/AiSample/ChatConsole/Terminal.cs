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

    // 選択肢のキーを 1 つ読む (Enter は '\r')。入力がリダイレクトされているときは行で読む (空行は Enter、EOF は 'q')
    public static char ReadChoice(string label, string keys)
    {
        Console.Write(label);
        while (true)
        {
            char c;
            if (Console.IsInputRedirected)
            {
                var line = Console.ReadLine();
                if (line is null)
                {
                    Console.WriteLine();
                    return 'q';
                }

                c = line.Length == 0 ? '\r' : Char.ToLowerInvariant(line[0]);
            }
            else
            {
                var key = Console.ReadKey(true);
                c = key.Key == ConsoleKey.Enter ? '\r' : Char.ToLowerInvariant(key.KeyChar);
            }

            if (keys.Contains(c, StringComparison.Ordinal))
            {
                Console.WriteLine();
                return c;
            }
        }
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
