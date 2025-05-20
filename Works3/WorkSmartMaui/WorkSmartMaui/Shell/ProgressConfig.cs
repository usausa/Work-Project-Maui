namespace WorkSmartMaui.Shell;

public sealed class ProgressConfig
{
    public Color BackgroundColor { get; set; } = new(0, 0, 0, 64);

    public Color MessageBackgroundColor { get; set; } = new(0, 0, 0, 128);
    public Color MessageColor { get; set; } = Colors.White;
    public float MessageHeight { get; set; } = 48;
    public float MessageSideMargin { get; set; } = 16;
    public float MessageCornerRadius { get; set; } = 6;
    public float MessageFontSize { get; set; } = 14;

    public Color ProgressAreaBackgroundColor { get; set; } = new(0, 0, 0, 128);
    public Color ProgressCircleColor1 { get; set; } = Colors.White;
    public Color ProgressCircleColor2 { get; set; } = Colors.Gray;
    public Color ProgressValueColor { get; set; } = Colors.White;
    public float ProgressAreaSize { get; set; } = 80;
    public float ProgressAreaCornerRadius { get; set; } = 16;
    public float ProgressSize { get; set; } = 64;
    public float ProgressWidth { get; set; } = 8;
    public float ProgressValueFontSize { get; set; } = 28;
}

