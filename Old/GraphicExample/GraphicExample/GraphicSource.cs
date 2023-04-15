namespace GraphicExample;

public sealed class GraphicSource<T> : IGraphicSource
{
    public event EventHandler<EventArgs>? InvalidateRequest;

    object IGraphicSource.Source => Value!;

    public T Value { get; }

    public GraphicSource(T value)
    {
        Value = value;
    }

    public void Update()
    {
        InvalidateRequest?.Invoke(this, EventArgs.Empty);
    }
}
