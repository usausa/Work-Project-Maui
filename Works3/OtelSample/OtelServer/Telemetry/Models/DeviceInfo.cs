namespace OtelServer.Telemetry.Models;

// 端末 (device.id) ごとの最新のリソース属性と件数
public sealed record DeviceInfo(
    string DeviceId,
    string ServiceName,
    string? ServiceVersion,
    string? Model,
    string? OsName,
    string? OsVersion,
    DateTimeOffset FirstSeen,
    DateTimeOffset LastSeen,
    int LogCount,
    int SpanCount,
    int MetricCount);

public sealed record TelemetrySummary(
    int ServiceCount,
    int DeviceCount,
    int MetricCount,
    int LogCount,
    int SpanCount,
    int TraceCount,
    DateTimeOffset? LastReceivedAt);

// 受信件数の推移 (1 分単位)
public sealed record IngestPoint(DateTimeOffset Time, int Count);

public sealed record IngestHistory(
    IReadOnlyList<IngestPoint> Logs,
    IReadOnlyList<IngestPoint> Spans,
    IReadOnlyList<IngestPoint> Metrics);

// 一覧の絞り込み条件
public sealed record LogQuery(
    string? ServiceName = null,
    string? DeviceId = null,
    int MinSeverity = 0,
    string? Text = null,
    string? TraceId = null,
    int MaxCount = 500);

public sealed record TraceQuery(
    string? ServiceName = null,
    string? DeviceId = null,
    bool ErrorsOnly = false,
    string? Text = null,
    int MaxCount = 200);
