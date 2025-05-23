namespace Template.MobileApp.Shell;

public sealed class ProgressView : IProgressView, IProgressDrawer, IProgressStrategyUpdate, IProgressStrategyCallback
{
    private readonly ProgressConfig config;

    private ProgressOverlay? overlay;

    private IProgressStrategy? current;

    private bool visible;

    public ProgressView(ProgressConfig config)
    {
        this.config = config;
    }

    void IProgressDrawer.Draw(ICanvas canvas, RectF dirtyRect)
    {
        canvas.FillColor = config.BackgroundColor;
        canvas.FillRectangle(dirtyRect);

        current?.Draw(canvas, dirtyRect);
    }

    private ProgressOverlay GetOverlay()
    {
        overlay ??= new ProgressOverlay(Application.Current!.Windows[0], this);
        return overlay;
    }

    void IProgressView.Show()
    {
        if (visible)
        {
            return;
        }

        var o = GetOverlay();
        if (o.Window.AddOverlay(o))
        {
            visible = true;
        }

        current = null;
    }

    void IProgressView.Hide()
    {
        if (!visible)
        {
            return;
        }

        var o = GetOverlay();
        if (o.Window.RemoveOverlay(o))
        {
            visible = false;
        }

        current?.Detach();
        current = null;
    }

    void IProgressStrategyUpdate.UpdateStrategy(IProgressStrategy? strategy)
    {
        current?.Detach();
        current = strategy;
        current?.Attach(this);

        GetOverlay().Invalidate();
    }

    void IProgressStrategyCallback.Invalidate()
    {
        GetOverlay().Invalidate();
    }
}
