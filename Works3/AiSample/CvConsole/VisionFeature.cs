namespace CvConsole;

// アプリの Sample > CV Net の 4 画面 (Object / Tag / People / Ocr) に対応する解析の種類
internal enum VisionFeature
{
    Object,
    Tag,
    People,
    Ocr
}

internal static class VisionFeatures
{
    public static readonly VisionFeature[] All = Enum.GetValues<VisionFeature>();

    public static string Names => String.Join(" / ", All) + " / All";

    // All は全種類
    public static bool TryParse(string text, out VisionFeature[] features)
    {
        if (text.Equals("All", StringComparison.OrdinalIgnoreCase))
        {
            features = All;
            return true;
        }

        if (Enum.TryParse<VisionFeature>(text, true, out var feature) && Enum.IsDefined(feature))
        {
            features = [feature];
            return true;
        }

        features = [];
        return false;
    }
}
