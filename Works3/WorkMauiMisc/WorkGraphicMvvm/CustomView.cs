namespace WorkGraphicMvvm;

public sealed class CustomView : GraphicsView, IDrawable
{
    public static readonly BindableProperty CustomColorProperty = BindableProperty.Create(
        nameof(CustomColor),
        typeof(Color),
        typeof(CustomView),
        Colors.Transparent,
        BindingMode.TwoWay,
        propertyChanged: HandlePropertyChanged);

    public Color CustomColor
    {
        get => (Color)GetValue(CustomColorProperty);
        set => SetValue(CustomColorProperty, value);
    }

    public CustomView()
    {
        Drawable = this;
    }

    private static void HandlePropertyChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (oldValue == newValue)
        {
            return;
        }

        var control = (CustomView)bindable;
        control.Invalidate();
    }

    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        canvas.Antialias = true;
        canvas.StrokeColor = CustomColor;
        canvas.StrokeSize = 5;
        canvas.StrokeLineCap = LineCap.Round;
        canvas.DrawLine((float)(Width / 4), (float)(Height / 4), (float)(Width * 3 / 4), (float)(Height * 3 / 4));
    }
}
