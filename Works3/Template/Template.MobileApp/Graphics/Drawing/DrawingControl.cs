namespace Template.MobileApp.Graphics.Drawing;

public sealed class DrawingControl : GraphicsView
{
    public static readonly BindableProperty DrawingProperty = BindableProperty.Create(
        nameof(Drawing),
        typeof(IDrawingObject),
        typeof(DrawingControl),
        propertyChanged: HandlePropertyChanged);

    public IDrawingObject Drawing
    {
        get => (IDrawingObject)GetValue(DrawingProperty);
        set => SetValue(DrawingProperty, value);
    }

    private static void HandlePropertyChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (oldValue == newValue)
        {
            return;
        }

        ((DrawingControl)bindable).HandlePropertyChanged(oldValue as IDrawingObject, newValue as IDrawingObject);
    }

    private void HandlePropertyChanged(IDrawingObject? oldValue, IDrawingObject? newValue)
    {
        if (oldValue is not null)
        {
            oldValue.Detach();
            Drawable = null!;
        }
        if (newValue is not null)
        {
            newValue.Attach(this);
            Drawable = newValue;
        }
    }
}
