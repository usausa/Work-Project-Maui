namespace Template.MobileApp.Controls;

using SkiaSharp.Views.Maui;

using Template.MobileApp.Models.Sample;

//--------------------------------------------------------------------------------
// Sleep
//--------------------------------------------------------------------------------
// 睡眠の段階の帯。上から 覚醒・レム・浅い・深い の段に、時間の長さで塗る。下に 1 時間ごとの時刻
public sealed class KitSleepBand : SKCanvasView
{
    private const float LabelWidth = 36f;

    private const float AxisHeight = 20f;

    private const float FontSize = 11f;

    private static readonly string[] LaneNames = ["覚醒", "レム", "浅い", "深い"];

    private static readonly SKTypeface JapaneseTypeface = SKFontManager.Default.MatchCharacter('睡') ?? SKTypeface.Default;

    public static readonly BindableProperty SpansProperty = BindableProperty.Create(
        nameof(Spans),
        typeof(IReadOnlyList<KitSleepSpan>),
        typeof(KitSleepBand),
        propertyChanged: OnVisualChanged);

    public IReadOnlyList<KitSleepSpan>? Spans
    {
        get => (IReadOnlyList<KitSleepSpan>?)GetValue(SpansProperty);
        set => SetValue(SpansProperty, value);
    }

    public static readonly BindableProperty AwakeColorProperty = BindableProperty.Create(
        nameof(AwakeColor),
        typeof(Color),
        typeof(KitSleepBand),
        Colors.Orange,
        propertyChanged: OnVisualChanged);

    public Color AwakeColor
    {
        get => (Color)GetValue(AwakeColorProperty);
        set => SetValue(AwakeColorProperty, value);
    }

    public static readonly BindableProperty RemColorProperty = BindableProperty.Create(
        nameof(RemColor),
        typeof(Color),
        typeof(KitSleepBand),
        Colors.MediumPurple,
        propertyChanged: OnVisualChanged);

    public Color RemColor
    {
        get => (Color)GetValue(RemColorProperty);
        set => SetValue(RemColorProperty, value);
    }

    public static readonly BindableProperty LightColorProperty = BindableProperty.Create(
        nameof(LightColor),
        typeof(Color),
        typeof(KitSleepBand),
        Colors.LightSkyBlue,
        propertyChanged: OnVisualChanged);

    public Color LightColor
    {
        get => (Color)GetValue(LightColorProperty);
        set => SetValue(LightColorProperty, value);
    }

    public static readonly BindableProperty DeepColorProperty = BindableProperty.Create(
        nameof(DeepColor),
        typeof(Color),
        typeof(KitSleepBand),
        Colors.DarkSlateBlue,
        propertyChanged: OnVisualChanged);

    public Color DeepColor
    {
        get => (Color)GetValue(DeepColorProperty);
        set => SetValue(DeepColorProperty, value);
    }

    public static readonly BindableProperty TextColorProperty = BindableProperty.Create(
        nameof(TextColor),
        typeof(Color),
        typeof(KitSleepBand),
        Colors.Gray,
        propertyChanged: OnVisualChanged);

    public Color TextColor
    {
        get => (Color)GetValue(TextColorProperty);
        set => SetValue(TextColorProperty, value);
    }

    public static readonly BindableProperty GridColorProperty = BindableProperty.Create(
        nameof(GridColor),
        typeof(Color),
        typeof(KitSleepBand),
        Colors.LightGray,
        propertyChanged: OnVisualChanged);

    public Color GridColor
    {
        get => (Color)GetValue(GridColorProperty);
        set => SetValue(GridColorProperty, value);
    }

    private static void OnVisualChanged(BindableObject bindable, object? oldValue, object? newValue) =>
        ((KitSleepBand)bindable).InvalidateSurface();

    protected override void OnPaintSurface(SKPaintSurfaceEventArgs e)
    {
        var canvas = e.Surface.Canvas;
        canvas.Clear();

        var spans = Spans;
        if ((spans is null) || (spans.Count == 0) || (Width <= 0))
        {
            return;
        }

        var density = (float)(e.Info.Width / Width);
        var left = LabelWidth * density;
        var bottom = e.Info.Height - (AxisHeight * density);
        var laneHeight = bottom / LaneNames.Length;
        var start = spans[0].Start;
        var end = spans[^1].Start + spans[^1].Length;
        var total = (float)(end - start).TotalMinutes;
        var width = e.Info.Width - left;

        float ToX(DateTime time) => left + ((float)(time - start).TotalMinutes / total * width);

        using var font = new SKFont(JapaneseTypeface, FontSize * density);
        using var text = new SKPaint();
        text.IsAntialias = true;
        text.Color = TextColor.ToSKColor();
        using var grid = new SKPaint();
        grid.Color = GridColor.ToSKColor();
        grid.StrokeWidth = density;

        // 段の名前と区切りの線
        for (var lane = 0; lane < LaneNames.Length; lane++)
        {
            var center = (lane + 0.5f) * laneHeight;
            canvas.DrawLine(left, center, e.Info.Width, center, grid);
            canvas.DrawText(LaneNames[lane], 0, center + (font.Size * 0.35f), SKTextAlign.Left, font, text);
        }

        // 段階ごとの塗り
        using var fill = new SKPaint();
        fill.IsAntialias = true;
        foreach (var span in spans)
        {
            var lane = (int)span.Stage;
            var x0 = ToX(span.Start);
            var x1 = ToX(span.Start + span.Length);
            fill.Color = ColorOf(span.Stage).ToSKColor();
            canvas.DrawRoundRect(
                new SKRect(x0, (lane + 0.18f) * laneHeight, Math.Max(x0 + density, x1), (lane + 0.82f) * laneHeight),
                2 * density,
                2 * density,
                fill);
        }

        // 1 時間ごとの時刻 (端に近いものは描かない)
        var hour = start.Date.AddHours(start.Hour + 1);
        while (hour < end)
        {
            var x = ToX(hour);
            if ((x - left > 16 * density) && (e.Info.Width - x > 16 * density))
            {
                canvas.DrawLine(x, bottom, x, bottom + (3 * density), grid);
                canvas.DrawText($"{hour.Hour}時", x, e.Info.Height - (4 * density), SKTextAlign.Center, font, text);
            }
            hour = hour.AddHours(1);
        }
    }

    private Color ColorOf(KitSleepStage stage) => stage switch
    {
        KitSleepStage.Awake => AwakeColor,
        KitSleepStage.Rem => RemColor,
        KitSleepStage.Light => LightColor,
        _ => DeepColor
    };
}
