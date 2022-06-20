namespace OverlayExample;

public sealed class LoadingOverlay : WindowOverlay
{
    public LoadingOverlay(IWindow window)
        : base(window)
    {
        AddWindowElement(new LoadingElementOverlay());

        EnableDrawableTouchHandling = true;
    }

    private class LoadingElementOverlay : IWindowOverlayElement
    {
        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            canvas.FillColor = Color.FromRgba(0, 0, 0, 64);
            canvas.FillRectangle(dirtyRect);
        }

        public bool Contains(Point point) => true;
    }
}
