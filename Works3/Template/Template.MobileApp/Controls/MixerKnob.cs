namespace Template.MobileApp.Controls;

public sealed class MixerKnob : GraphicsView, IDrawable
{
    private const float StartAngle = 225f;

    private const float SweepAngle = 270f;

    private const float GapAngle = (360f - SweepAngle) / 2;

    //--------------------------------------------------------------------------------
    // Property
    //--------------------------------------------------------------------------------

    public static readonly BindableProperty ValueProperty = BindableProperty.Create(
        nameof(Value),
        typeof(double),
        typeof(MixerKnob),
        0.0,
        defaultBindingMode: BindingMode.TwoWay,
        propertyChanged: (b, _, _) => ((MixerKnob)b).Invalidate());

    public static readonly BindableProperty MinimumProperty = BindableProperty.Create(
        nameof(Minimum),
        typeof(double),
        typeof(MixerKnob),
        0.0);

    public static readonly BindableProperty MaximumProperty = BindableProperty.Create(
        nameof(Maximum),
        typeof(double),
        typeof(MixerKnob),
        100.0);

    public static readonly BindableProperty KnobBackgroundProperty = BindableProperty.Create(
        nameof(KnobBackground),
        typeof(Brush),
        typeof(MixerKnob),
        Brush.LightGray,
        propertyChanged: OnPropertyChanged);

    public static readonly BindableProperty PointerBackgroundProperty = BindableProperty.Create(
        nameof(PointerBackground),
        typeof(Brush),
        typeof(MixerKnob),
        Brush.Black,
        propertyChanged: OnPropertyChanged);

    public static readonly BindableProperty IndicatorColorProperty = BindableProperty.Create(
        nameof(IndicatorColor),
        typeof(Color),
        typeof(MixerKnob),
        Colors.Orange,
        propertyChanged: OnPropertyChanged);

    public static readonly BindableProperty IndicatorBackgroundColorProperty = BindableProperty.Create(
        nameof(IndicatorBackgroundColor),
        typeof(Color),
        typeof(MixerKnob),
        Colors.Snow,
        propertyChanged: OnPropertyChanged);

    public static readonly BindableProperty IndicatorWidthProperty = BindableProperty.Create(
        nameof(IndicatorWidth),
        typeof(double),
        typeof(MixerKnob),
        8.0,
        propertyChanged: OnPropertyChanged);

    public static readonly BindableProperty IndicatorMarginProperty = BindableProperty.Create(
        nameof(IndicatorMargin),
        typeof(double),
        typeof(MixerKnob),
        2.0,
        propertyChanged: OnPropertyChanged);

    public static readonly BindableProperty PointerWidthProperty = BindableProperty.Create(
        nameof(PointerWidth),
        typeof(double),
        typeof(MixerKnob),
        12.0,
        propertyChanged: OnPropertyChanged);

    public static readonly BindableProperty PointerMarginProperty = BindableProperty.Create(
        nameof(PointerMargin),
        typeof(double),
        typeof(MixerKnob),
        4.0,
        propertyChanged: OnPropertyChanged);

    public double Value
    {
        get => (double)GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    public double Minimum
    {
        get => (double)GetValue(MinimumProperty);
        set => SetValue(MinimumProperty, value);
    }

    public double Maximum
    {
        get => (double)GetValue(MaximumProperty);
        set => SetValue(MaximumProperty, value);
    }

    public Brush KnobBackground
    {
        get => (Brush)GetValue(KnobBackgroundProperty);
        set => SetValue(KnobBackgroundProperty, value);
    }

    public Brush PointerBackground
    {
        get => (Brush)GetValue(PointerBackgroundProperty);
        set => SetValue(PointerBackgroundProperty, value);
    }

    public Color IndicatorColor
    {
        get => (Color)GetValue(IndicatorColorProperty);
        set => SetValue(IndicatorColorProperty, value);
    }

    public Color IndicatorBackgroundColor
    {
        get => (Color)GetValue(IndicatorBackgroundColorProperty);
        set => SetValue(IndicatorBackgroundColorProperty, value);
    }

    public double IndicatorWidth
    {
        get => (double)GetValue(IndicatorWidthProperty);
        set => SetValue(IndicatorWidthProperty, value);
    }

    public double IndicatorMargin
    {
        get => (double)GetValue(IndicatorMarginProperty);
        set => SetValue(IndicatorMarginProperty, value);
    }

    public double PointerWidth
    {
        get => (double)GetValue(PointerWidthProperty);
        set => SetValue(PointerWidthProperty, value);
    }

    public double PointerMargin
    {
        get => (double)GetValue(PointerMarginProperty);
        set => SetValue(PointerMarginProperty, value);
    }

    //--------------------------------------------------------------------------------
    // Constructor
    //--------------------------------------------------------------------------------

    public MixerKnob()
    {
        Drawable = this;

        StartInteraction += (_, e) => UpdateValue(e.Touches[0]);
        DragInteraction += (_, e) => UpdateValue(e.Touches[0]);
    }

    //--------------------------------------------------------------------------------
    // Render
    //--------------------------------------------------------------------------------

    private static void OnPropertyChanged(BindableObject bindable, object oldValue, object newValue)
    {
        ((MixerKnob)bindable).Invalidate();
    }

    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        var cx = dirtyRect.Center.X;
        var cy = dirtyRect.Center.Y;
        var size = Math.Min(dirtyRect.Width, dirtyRect.Height);

        var valuePercent = (Value - Minimum) / (Maximum - Minimum);
        var valueSweepAngle = (float)(SweepAngle * valuePercent);

        var indicatorWidth = (float)IndicatorWidth;
        var indicatorSize = size - indicatorWidth;
        var indicatorRadius = indicatorSize / 2;
        var indicatorRect = new RectF(cx - indicatorRadius, cy - indicatorRadius, indicatorSize, indicatorSize);

        canvas.StrokeLineCap = LineCap.Butt;
        canvas.StrokeSize = indicatorWidth;
        canvas.StrokeColor = IndicatorBackgroundColor;
        canvas.DrawArc(indicatorRect, StartAngle, StartAngle - SweepAngle, true, false);

        canvas.StrokeColor = IndicatorColor;
        canvas.DrawArc(indicatorRect, StartAngle, StartAngle - valueSweepAngle, true, false);

        var knobSize = size - (indicatorWidth * 2) - ((float)IndicatorMargin * 2);
        var knobRadius = knobSize / 2;

        canvas.SetFillPaint(KnobBackground, new RectF(cx - knobRadius, cy - knobRadius, knobSize, knobSize));
        canvas.FillCircle(cx, cy, knobRadius);

        var pointerSize = (float)PointerWidth;
        var pointerRadius = pointerSize / 2;

        var pointerDistance = indicatorRadius - pointerSize - PointerMargin;
        var pointerRadians = Math.PI * (valueSweepAngle - StartAngle) / 180.0;
        var pointerX = (float)(cx + (Math.Cos(pointerRadians) * pointerDistance));
        var pointerY = (float)(cy + (Math.Sin(pointerRadians) * pointerDistance));

        canvas.SetFillPaint(PointerBackground, new RectF(pointerX - pointerRadius, pointerY - pointerRadius, pointerSize, pointerSize));
        canvas.FillCircle(pointerX, pointerY, pointerRadius);
    }

    //--------------------------------------------------------------------------------
    // Touch
    //--------------------------------------------------------------------------------

    private void UpdateValue(PointF point)
    {
        var center = new Point(Width / 2, Height / 2);
        var dx = point.X - center.X;
        var dy = point.Y - center.Y;

        var angle = Math.Atan2(dy, dx) * 180 / Math.PI;
        angle = Math.Clamp(angle > 90 ? angle - 90 : angle + 270, GapAngle, 360 - GapAngle);
        angle -= GapAngle;

        var percent = angle / SweepAngle;

        Value = Minimum + (percent * (Maximum - Minimum));
    }
}
