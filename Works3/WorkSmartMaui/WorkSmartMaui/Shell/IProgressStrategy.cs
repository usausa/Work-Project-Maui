namespace WorkSmartMaui.Shell;

public interface IProgressStrategyCallback
{
    void Invalidate();
}

public interface IProgressStrategy
{
    void Attach(IProgressStrategyCallback value);

    void Detach();

    void Draw(ICanvas canvas, RectF dirtyRect);
}
