namespace WorkSmartMaui.Shell;

public sealed class ProgressOverlay : WindowOverlay
{
    public ProgressOverlay(IWindow window, IProgressDrawer drawer)
        : base(window)
    {
        AddWindowElement(new OverlayElement(drawer));
        EnableDrawableTouchHandling = true;
    }

    private sealed class OverlayElement : IWindowOverlayElement
    {
        private readonly IProgressDrawer drawer;

        public OverlayElement(IProgressDrawer drawer)
        {
            this.drawer = drawer;
        }

        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            drawer.Draw(canvas, dirtyRect);
        }

        public bool Contains(Point point) => true;
    }
}

