namespace OtelServer.Telemetry.Models;

public enum MetricKind
{
    Gauge,
    Sum,
    Histogram,
    ExponentialHistogram,
    Summary
}

// メトリクスのデータポイント 1 件。ヒストグラムは Value = 平均 (sum / count) で、Count / Min / Max を別に持つ
public sealed record MetricPoint(
    DateTimeOffset ReceivedAt,
    ResourceInfo Resource,
    string ScopeName,
    string Name,
    string? Description,
    string? Unit,
    MetricKind Kind,
    DateTimeOffset Timestamp,
    double Value,
    long? Count,
    double? Min,
    double? Max,
    IReadOnlyList<KeyValueAttr> Attributes);

public sealed record MetricSeriesSnapshot(
    string ServiceName,
    string MetricName,
    string? Unit,
    string? Description,
    MetricKind Kind,
    IReadOnlyList<MetricPoint> Points);
