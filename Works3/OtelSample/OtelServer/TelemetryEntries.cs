namespace OtelServer;

// 受信したテレメトリを画面向けに平坦化したもの (JSON で返す)

internal sealed record LogEntry(
    DateTimeOffset Time,
    string Service,
    string Device,
    string Severity,
    string Body,
    string? TraceId,
    IReadOnlyDictionary<string, string> Attributes);

internal sealed record SpanEntry(
    DateTimeOffset Time,
    string Service,
    string Device,
    string Name,
    double DurationMs,
    string Status,
    string TraceId,
    string SpanId,
    string? ParentSpanId,
    IReadOnlyDictionary<string, string> Attributes);

internal sealed record MetricEntry(
    DateTimeOffset Time,
    string Service,
    string Device,
    string Name,
    string Unit,
    string Type,
    string Value,
    IReadOnlyDictionary<string, string> Attributes);

internal sealed record Summary(
    int Logs,
    int Spans,
    int Metrics,
    int Batches,
    DateTimeOffset? LastReceived,
    IReadOnlyList<string> Devices);
