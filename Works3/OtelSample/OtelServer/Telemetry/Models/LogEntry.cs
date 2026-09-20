namespace OtelServer.Telemetry.Models;

public sealed record LogEntry(
    DateTimeOffset ReceivedAt,
    ResourceInfo Resource,
    string ScopeName,
    DateTimeOffset Timestamp,
    string SeverityText,
    int SeverityNumber,
    string Body,
    string? TraceId,
    string? SpanId,
    IReadOnlyList<KeyValueAttr> Attributes);
