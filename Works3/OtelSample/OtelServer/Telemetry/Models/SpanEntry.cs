namespace OtelServer.Telemetry.Models;

public sealed record SpanEntry(
    DateTimeOffset ReceivedAt,
    ResourceInfo Resource,
    string ScopeName,
    string TraceId,
    string SpanId,
    string? ParentSpanId,
    string Name,
    string Kind,
    DateTimeOffset StartTime,
    DateTimeOffset EndTime,
    string StatusCode,
    string? StatusMessage,
    IReadOnlyList<KeyValueAttr> Attributes)
{
    public TimeSpan Duration => EndTime - StartTime;
}
