namespace WorkGraphicBasic;

public sealed class TimelineCellView : GraphicsView, IDrawable
{
    public static readonly BindableProperty ValueProperty = BindableProperty.Create(
        nameof(Value),
        typeof(int),
        typeof(TimelineCellView),
        0,
        BindingMode.TwoWay,
        propertyChanged: HandlePropertyChanged);

    public int Value
    {
        get => (int)GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    public TimelineCellView()
    {
        Drawable = this;
    }

    private static void HandlePropertyChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (oldValue == newValue)
        {
            return;
        }

        var control = (TimelineCellView)bindable;
        control.Invalidate();
    }

    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        // TODO
        canvas.Antialias = true;
        canvas.StrokeColor = Colors.Red;
        canvas.StrokeSize = 5;
        canvas.StrokeLineCap = LineCap.Round;
        canvas.DrawLine(100, 0, 100, (float)Height);
    }
}
