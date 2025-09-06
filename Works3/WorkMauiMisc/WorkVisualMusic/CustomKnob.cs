namespace WorkVisualMusic;

public sealed class CustomKnob : GraphicsView, IDrawable
{
    // Value

    public static readonly BindableProperty ValueProperty = BindableProperty.Create(
        nameof(Value),
        typeof(double),
        typeof(CustomKnob),
        0.0,
        propertyChanged: (b, _, _) => ((CustomKnob)b).Invalidate());

    public double Value
    {
        get => (double)GetValue(ValueProperty);
        set => SetValue(ValueProperty, Math.Clamp(value, Minimum, Maximum));
    }

    public static readonly BindableProperty MinimumProperty = BindableProperty.Create(
        nameof(Minimum),
        typeof(double),
        typeof(CustomKnob),
        0.0);

    public double Minimum
    {
        get => (double)GetValue(MinimumProperty);
        set => SetValue(MinimumProperty, value);
    }

    public static readonly BindableProperty MaximumProperty = BindableProperty.Create(
        nameof(Maximum),
        typeof(double),
        typeof(CustomKnob),
        100.0);

    public double Maximum
    {
        get => (double)GetValue(MaximumProperty);
        set => SetValue(MaximumProperty, value);
    }

    public CustomKnob()
    {
        Drawable = this;

        var gesture = new PanGestureRecognizer();
        gesture.PanUpdated += OnPanUpdated;
        GestureRecognizers.Add(gesture);
    }

    private void OnPanUpdated(object? sender, PanUpdatedEventArgs e)
    {
    }

    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
    }
}
