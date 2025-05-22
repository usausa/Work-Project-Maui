namespace WorkSmartMaui.Shell;

public sealed class ProgressConfig
{
    public Color BackgroundColor { get; set; } = new(0, 0, 0, 64);

    public Color IndicatorColor { get; set; } = Colors.DeepSkyBlue;
    public float IndicatorSize { get; set; } = 32;

    public Color MessageBackgroundColor { get; set; } = Colors.White;
    public Color MessageColor { get; set; } = Colors.Black;
    public float MessageHeight { get; set; } = 48;
    public float MessageSideMargin { get; set; } = 16;
    public float MessageCornerRadius { get; set; } = 6;
    public float MessageFontSize { get; set; } = 14;

    public Color RateAreaBackgroundColor { get; set; } = Colors.White;
    public Color RateCircleColor1 { get; set; } = Colors.DeepSkyBlue;
    public Color RateCircleColor2 { get; set; } = Colors.Gray;
    public Color RateValueColor { get; set; } = Colors.Black;
    public float RateAreaSize { get; set; } = 80;
    public float RateAreaCornerRadius { get; set; } = 16;
    public float RateSize { get; set; } = 64;
    public float RateWidth { get; set; } = 8;
    public float RateValueFontSize { get; set; } = 28;
}
