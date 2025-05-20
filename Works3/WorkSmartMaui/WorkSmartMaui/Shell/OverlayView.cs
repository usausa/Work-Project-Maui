namespace WorkSmartMaui.Shell;

using System.Diagnostics.CodeAnalysis;

public sealed class OverlayView : WindowOverlay, IOverlayCallback
{
    private readonly OverlayElement element;

    [field: AllowNull, MaybeNull]
    public static OverlayView Instance
    {
        get
        {
            field ??= new OverlayView();
            return field;
        }
    }

    private OverlayView()
        : base(Application.Current!.Windows[0])
    {
        element = new OverlayElement();
        AddWindowElement(element);
        EnableDrawableTouchHandling = true;
    }

    public void Show()
    {
        element.UpdateStrategy(null);

        Window.AddOverlay(this);
    }

    public void Hide()
    {
        Window.RemoveOverlay(this);

        element.UpdateStrategy(null);
    }

    public void UpdateStrategy(IOverlayStrategy? value)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            element.UpdateStrategy(value);
            value?.Attach(this);

            Invalidate();
        });
    }

    private sealed class OverlayElement : IWindowOverlayElement
    {
        private IOverlayStrategy? strategy;

        public void UpdateStrategy(IOverlayStrategy? value)
        {
            strategy?.Detach();
            strategy = value;
        }

        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            canvas.FillColor = new(255, 255, 255, 64);
            canvas.FillRectangle(dirtyRect);

            strategy?.Draw(canvas, dirtyRect);
        }

        public bool Contains(Point point) => true;
    }
}

