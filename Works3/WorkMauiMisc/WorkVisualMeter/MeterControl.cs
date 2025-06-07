namespace WorkVisualMeter;

public sealed class MeterControl : GraphicsView, IDrawable
{
    public static readonly BindableProperty GaugeColorProperty = BindableProperty.Create(
        nameof(GaugeColor),
        typeof(Color),
        typeof(MeterControl),
        Colors.White);

    public Color GaugeColor
    {
        get => (Color)GetValue(GaugeColorProperty);
        set => SetValue(GaugeColorProperty, value);
    }

    public static readonly BindableProperty ValueColorProperty = BindableProperty.Create(
        nameof(ValueColor),
        typeof(Color),
        typeof(MeterControl),
        Colors.LightGreen);

    public Color ValueColor
    {
        get => (Color)GetValue(ValueColorProperty);
        set => SetValue(ValueColorProperty, value);
    }

    public static readonly BindableProperty BorderMarginProperty = BindableProperty.Create(
        nameof(BorderMargin),
        typeof(float),
        typeof(MeterControl),
        10f);

    public float BorderMargin
    {
        get => (float)GetValue(BorderMarginProperty);
        set => SetValue(BorderMarginProperty, value);
    }

    public static readonly BindableProperty GaugeWidthProperty = BindableProperty.Create(
        nameof(GaugeWidth),
        typeof(float),
        typeof(MeterControl),
        32f);

    public float GaugeWidth
    {
        get => (float)GetValue(GaugeWidthProperty);
        set => SetValue(GaugeWidthProperty, value);
    }

    public static readonly BindableProperty SpeedProperty = BindableProperty.Create(
        nameof(Speed),
        typeof(int),
        typeof(MeterControl));

    public int Speed
    {
        get => (int)GetValue(SpeedProperty);
        set => SetValue(SpeedProperty, value);
    }

    public static readonly BindableProperty MaxSpeedProperty = BindableProperty.Create(
        nameof(MaxSpeed),
        typeof(int),
        typeof(MeterControl));

    public int MaxSpeed
    {
        get => (int)GetValue(MaxSpeedProperty);
        set => SetValue(MaxSpeedProperty, value);
    }

    public MeterControl()
    {
        Drawable = this;
    }

    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        var width = dirtyRect.Width;
        var height = dirtyRect.Height;
        var cx = width / 2f;
        var cy = height / 2f;
        var radius = (Math.Min(dirtyRect.Width, dirtyRect.Height) / 2) - BorderMargin;

        var gaugeWidth = GaugeWidth;
        var strokeMargin = gaugeWidth / 2;
        var gaugeSize = radius * 2 - gaugeWidth;
        var gaugeRect = new RectF(cx - radius + strokeMargin, cy - radius + strokeMargin, gaugeSize, gaugeSize);

        canvas.Antialias = true;

        // Background
        canvas.FillColor = BackgroundColor;
        canvas.FillRectangle(0, 0, width, height);


        const float startAngle = 210f;
        const float gaugeAngle = 240f;
        var valueAngle = startAngle - (gaugeAngle * Speed / MaxSpeed);

        // Gauge
        canvas.StrokeColor = GaugeColor;
        canvas.StrokeSize = gaugeWidth;

        canvas.DrawArc(gaugeRect, startAngle, startAngle - gaugeAngle, true, false);

        // Value
        canvas.StrokeColor = ValueColor;
        canvas.StrokeSize = gaugeWidth;

        canvas.DrawArc(gaugeRect, startAngle, valueAngle, true, false);
    }
}
