namespace WorkEffect;

public static class InputRule
{
    public static Func<string, bool> Integer = s => String.IsNullOrEmpty(s) || Int32.TryParse(s, out _);
}
