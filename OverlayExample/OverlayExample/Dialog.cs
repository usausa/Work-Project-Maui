namespace OverlayExample;

public interface IDialog
{
    IDisposable Loading();
}

public sealed class Dialog : IDialog
{
    // Simple
    public IDisposable Loading()
    {
        var window = Application.Current!.MainPage!.GetParentWindow();
        return new SimpleOverlay(window);
    }

    // TODO Timer animation
    // TODO Progress
}

internal sealed class SimpleOverlay : WindowOverlay, IDisposable
{
    public SimpleOverlay(IWindow window)
        : base(window)
    {
        AddWindowElement(new LoadingElementOverlay());
        EnableDrawableTouchHandling = true;

        window.AddOverlay(this);
    }

    public void Dispose()
    {
        Window.RemoveOverlay(this);
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
