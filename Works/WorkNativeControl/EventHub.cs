namespace WorkNativeControl;

internal class EventHub
{
    public static EventHub Default { get; } = new();

    public EventHandler<EventArgs>? Handle { get; set; }
}
