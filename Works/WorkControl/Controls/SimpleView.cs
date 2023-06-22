using System.ComponentModel;

namespace WorkControl.Controls;

public interface ISimpleView : IView
{
    event EventHandler<EventArgs> Clicked;

    event EventHandler<int> PlatformCallRequested;

    Color Color { get; set; }

    void PerformClick();
}

public sealed class SimpleView : View, ISimpleView
{
    public event EventHandler<EventArgs> Clicked;

    public event EventHandler<int> PlatformCallRequested;

    public static BindableProperty ColorProperty = BindableProperty.Create(
        nameof(Color),
        typeof(Color),
        typeof(SimpleView),
        defaultBindingMode: BindingMode.OneWay);

    public Color Color
    {
        get => (Color)GetValue(ColorProperty);
        set => SetValue(ColorProperty, value);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public void PerformClick() => Clicked?.Invoke(this, EventArgs.Empty);

    public void PlatformCall(int value)
    {
        PlatformCallRequested?.Invoke(this, value);
        Handler?.Invoke(nameof(PlatformCallRequested), value);
    }
}
