namespace WorkNativeFocus;

public sealed class ForwardEventArgs : EventArgs
{
    public bool Forward { get; set; }
}


internal class EventHub
{
    public static EventHub Default { get; } = new();

    public EventHandler<ForwardEventArgs>? Handle { get; set; }
}
