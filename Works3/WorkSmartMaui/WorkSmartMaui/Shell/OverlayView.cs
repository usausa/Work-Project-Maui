using System.Diagnostics.CodeAnalysis;

namespace WorkSmartMaui.Shell;

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
        element = new OverlayElement(DefaultOverlayStrategy.Instance);
        AddWindowElement(element);
        EnableDrawableTouchHandling = true;
    }

    public void Show()
    {
        var strategy = DefaultOverlayStrategy.Instance;
        element.UpdateStrategy(strategy);
        strategy.Attach(this);

        Window.AddOverlay(this);
    }

    public void Hide()
    {
        var strategy = DefaultOverlayStrategy.Instance;
        element.UpdateStrategy(strategy);

        Window.RemoveOverlay(this);
    }

    public void UpdateStrategy(IOverlayStrategy? value)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            var strategy = value ?? DefaultOverlayStrategy.Instance;
            element.UpdateStrategy(strategy);
            strategy.Attach(this);

            Invalidate();
        });
    }

    private sealed class OverlayElement : IWindowOverlayElement
    {
        private IOverlayStrategy strategy;

        public OverlayElement(IOverlayStrategy strategy)
        {
            this.strategy = strategy;
        }

        public void UpdateStrategy(IOverlayStrategy value)
        {
            strategy.Detach();
            strategy = value;
        }

        public void Draw(ICanvas canvas, RectF dirtyRect) =>
            strategy.Draw(canvas, dirtyRect);

        public bool Contains(Point point) => true;
    }
}

