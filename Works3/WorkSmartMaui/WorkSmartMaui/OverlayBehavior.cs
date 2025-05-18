namespace WorkSmartMaui;

public static class OverlayBehavior
{
    public static readonly BindableProperty IsBusyProperty = BindableProperty.CreateAttached(
        "IsBusy",
        typeof(bool),
        typeof(OverlayBehavior),
        false,
        propertyChanged: HandleIsBusyChanged);

    public static bool GetIsBusy(BindableObject obj)
    {
        return (bool)obj.GetValue(IsBusyProperty);
    }

    public static void SetIsBusy(BindableObject obj, bool value)
    {
        obj.SetValue(IsBusyProperty, value);
    }

    private static void HandleIsBusyChanged(BindableObject bindable, object? oldValue, object? newValue)
    {
        if (oldValue == newValue)
        {
            return;
        }

        if (newValue is true)
        {
            if (overlay is null)
            {
                var window = Application.Current!.Windows[0];
                overlay = new LoadingOverlay(window);
            }
            overlay.Show();
        }
        else
        {
            overlay?.Hide();
        }
    }

    private static LoadingOverlay? overlay;

    private sealed class LoadingOverlay : WindowOverlay
    {
        public LoadingOverlay(IWindow window)
            : base(window)
        {
            AddWindowElement(new ElementOverlay());
            EnableDrawableTouchHandling = true;
        }

        public void Show()
        {
            Window.AddOverlay(this);
        }

        public void Hide()
        {
            Window.RemoveOverlay(this);
        }
    }


    private sealed class ElementOverlay : IWindowOverlayElement
    {
        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            canvas.FillColor = new(255, 255, 255, 64);
            canvas.FillRectangle(dirtyRect);
        }

        public bool Contains(Point point) => true;
    }
}
