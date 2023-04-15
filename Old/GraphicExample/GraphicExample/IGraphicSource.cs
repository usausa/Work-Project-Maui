namespace GraphicExample;

public interface IGraphicSource
{
    event EventHandler<EventArgs>? InvalidateRequest;

    object Source { get; }
}
