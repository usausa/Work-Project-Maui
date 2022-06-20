namespace OverlayExample;

public interface ITextProgress : IDisposable
{
    void Update(string text);
}

public interface IDialog
{
    IDisposable Loading();

    ITextProgress Loading(string text);
}

public sealed class Dialog : IDialog
{
    // Simple
    public IDisposable Loading()
    {
        var window = Application.Current!.MainPage!.GetParentWindow();
        return new SimpleOverlay(window);
    }

    public ITextProgress Loading(string text)
    {
        var window = Application.Current!.MainPage!.GetParentWindow();
        return new TextOverlay(window, text);
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

internal sealed class TextOverlay : WindowOverlay, ITextProgress
{
    private readonly LoadingElementOverlay overlay;

    public TextOverlay(IWindow window, string text)
        : base(window)
    {
        overlay = new LoadingElementOverlay { Text = text };
        AddWindowElement(overlay);
        EnableDrawableTouchHandling = true;

        window.AddOverlay(this);
    }

    public void Dispose()
    {
        Window.RemoveOverlay(this);
    }

    public void Update(string text)
    {
        //if (!MainThread.IsMainThread)
        //{
        //    MainThread.BeginInvokeOnMainThread(() => Update(text));
        //    return;
        //}

        overlay.Text = text;
        Invalidate();
    }

    private class LoadingElementOverlay : IWindowOverlayElement
    {
        public string Text { get; set; } = string.Empty;

        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            canvas.FillColor = Color.FromRgba(0, 0, 0, 64);
            canvas.FillRectangle(dirtyRect);
            canvas.FontSize = 48;
            canvas.DrawString(Text, dirtyRect, HorizontalAlignment.Center, VerticalAlignment.Center);
        }

        public bool Contains(Point point) => true;
    }
}
