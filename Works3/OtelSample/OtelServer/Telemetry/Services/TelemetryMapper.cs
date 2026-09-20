namespace OtelServer.Telemetry.Services;

using System.Globalization;

using Google.Protobuf;

using OpenTelemetry.Proto.Common.V1;
using OpenTelemetry.Proto.Logs.V1;
using OpenTelemetry.Proto.Metrics.V1;
using OpenTelemetry.Proto.Resource.V1;
using OpenTelemetry.Proto.Trace.V1;

using OtelServer.Telemetry.Models;

// OTLP のメッセージをストアのモデルへ平坦化する
internal static class TelemetryMapper
{
    public static IEnumerable<LogEntry> MapLogs(IEnumerable<ResourceLogs> resourceLogs, DateTimeOffset receivedAt)
    {
        foreach (var resourceLog in resourceLogs)
        {
            var resource = MapResource(resourceLog.Resource);
            foreach (var scope in resourceLog.ScopeLogs)
            {
                var scopeName = scope.Scope?.Name ?? string.Empty;
                foreach (var record in scope.LogRecords)
                {
                    yield return new LogEntry(
                        receivedAt,
                        resource,
                        scopeName,
                        FromUnixNano(record.TimeUnixNano != 0 ? record.TimeUnixNano : record.ObservedTimeUnixNano),
                        record.SeverityText.Length > 0 ? record.SeverityText : record.SeverityNumber.ToString(),
                        (int)record.SeverityNumber,
                        ToText(record.Body) ?? string.Empty,
                        ToHex(record.TraceId),
                        ToHex(record.SpanId),
                        MapAttributes(record.Attributes));
                }
            }
        }
    }

    public static IEnumerable<SpanEntry> MapSpans(IEnumerable<ResourceSpans> resourceSpans, DateTimeOffset receivedAt)
    {
        foreach (var resourceSpan in resourceSpans)
        {
            var resource = MapResource(resourceSpan.Resource);
            foreach (var scope in resourceSpan.ScopeSpans)
            {
                var scopeName = scope.Scope?.Name ?? string.Empty;
                foreach (var span in scope.Spans)
                {
                    yield return new SpanEntry(
                        receivedAt,
                        resource,
                        scopeName,
                        ToHex(span.TraceId) ?? string.Empty,
                        ToHex(span.SpanId) ?? string.Empty,
                        ToHex(span.ParentSpanId),
                        span.Name,
                        span.Kind.ToString(),
                        FromUnixNano(span.StartTimeUnixNano),
                        FromUnixNano(span.EndTimeUnixNano),
                        span.Status?.Code switch
                        {
                            Status.Types.StatusCode.Ok => "OK",
                            Status.Types.StatusCode.Error => "ERROR",
                            _ => "UNSET"
                        },
                        span.Status?.Message,
                        MapAttributes(span.Attributes));
                }
            }
        }
    }

    public static IEnumerable<MetricPoint> MapMetrics(IEnumerable<ResourceMetrics> resourceMetrics, DateTimeOffset receivedAt)
    {
        foreach (var resourceMetric in resourceMetrics)
        {
            var resource = MapResource(resourceMetric.Resource);
            foreach (var scope in resourceMetric.ScopeMetrics)
            {
                var scopeName = scope.Scope?.Name ?? string.Empty;
                foreach (var metric in scope.Metrics)
                {
                    foreach (var point in ExpandMetric(metric, receivedAt, resource, scopeName))
                    {
                        yield return point;
                    }
                }
            }
        }
    }

    // ヒストグラムは平均 (sum / count) を値にし、count / min / max を別に持つ
    private static IEnumerable<MetricPoint> ExpandMetric(Metric metric, DateTimeOffset receivedAt, ResourceInfo resource, string scopeName)
    {
        switch (metric.DataCase)
        {
            case Metric.DataOneofCase.Gauge:
                foreach (var point in metric.Gauge.DataPoints)
                {
                    yield return Build(metric, MetricKind.Gauge, point.TimeUnixNano, ToNumber(point), null, null, null, point.Attributes, receivedAt, resource, scopeName);
                }

                break;
            case Metric.DataOneofCase.Sum:
                foreach (var point in metric.Sum.DataPoints)
                {
                    yield return Build(metric, MetricKind.Sum, point.TimeUnixNano, ToNumber(point), null, null, null, point.Attributes, receivedAt, resource, scopeName);
                }

                break;
            case Metric.DataOneofCase.Histogram:
                foreach (var point in metric.Histogram.DataPoints)
                {
                    yield return Build(metric, MetricKind.Histogram, point.TimeUnixNano, Average(point.HasSum ? point.Sum : 0, point.Count), (long)point.Count, point.HasMin ? point.Min : null, point.HasMax ? point.Max : null, point.Attributes, receivedAt, resource, scopeName);
                }

                break;
            case Metric.DataOneofCase.ExponentialHistogram:
                foreach (var point in metric.ExponentialHistogram.DataPoints)
                {
                    yield return Build(metric, MetricKind.ExponentialHistogram, point.TimeUnixNano, Average(point.HasSum ? point.Sum : 0, point.Count), (long)point.Count, point.HasMin ? point.Min : null, point.HasMax ? point.Max : null, point.Attributes, receivedAt, resource, scopeName);
                }

                break;
            case Metric.DataOneofCase.Summary:
                foreach (var point in metric.Summary.DataPoints)
                {
                    yield return Build(metric, MetricKind.Summary, point.TimeUnixNano, Average(point.Sum, point.Count), (long)point.Count, null, null, point.Attributes, receivedAt, resource, scopeName);
                }

                break;
        }
    }

    private static MetricPoint Build(Metric metric, MetricKind kind, ulong time, double value, long? count, double? min, double? max, IEnumerable<KeyValue> attributes, DateTimeOffset receivedAt, ResourceInfo resource, string scopeName) =>
        new(
            receivedAt,
            resource,
            scopeName,
            metric.Name,
            metric.Description.Length > 0 ? metric.Description : null,
            metric.Unit.Length > 0 ? metric.Unit : null,
            kind,
            FromUnixNano(time),
            value,
            count,
            min,
            max,
            MapAttributes(attributes));

    private static double Average(double sum, ulong count) => count > 0 ? sum / count : sum;

    private static double ToNumber(NumberDataPoint point) => point.ValueCase switch
    {
        NumberDataPoint.ValueOneofCase.AsDouble => point.AsDouble,
        NumberDataPoint.ValueOneofCase.AsInt => point.AsInt,
        _ => 0d
    };

    private static ResourceInfo MapResource(Resource? resource) =>
        new(resource is null ? [] : MapAttributes(resource.Attributes));

    private static List<KeyValueAttr> MapAttributes(IEnumerable<KeyValue> attributes) =>
        attributes.Select(static x => new KeyValueAttr(x.Key, ToText(x.Value))).ToList();

    private static string? ToText(AnyValue? value) =>
        value?.ValueCase switch
        {
            AnyValue.ValueOneofCase.StringValue => value.StringValue,
            AnyValue.ValueOneofCase.BoolValue => value.BoolValue ? "true" : "false",
            AnyValue.ValueOneofCase.IntValue => value.IntValue.ToString(CultureInfo.InvariantCulture),
            AnyValue.ValueOneofCase.DoubleValue => value.DoubleValue.ToString("G", CultureInfo.InvariantCulture),
            AnyValue.ValueOneofCase.BytesValue => Convert.ToHexString(value.BytesValue.Span),
            AnyValue.ValueOneofCase.ArrayValue => "[" + String.Join(", ", value.ArrayValue.Values.Select(ToText)) + "]",
            AnyValue.ValueOneofCase.KvlistValue => "{" + String.Join(", ", value.KvlistValue.Values.Select(static x => $"{x.Key}={ToText(x.Value)}")) + "}",
            _ => null
        };

    private static DateTimeOffset FromUnixNano(ulong unixNano) =>
        unixNano == 0 ? DateTimeOffset.UtcNow : DateTimeOffset.UnixEpoch.AddTicks((long)(unixNano / 100UL));

    private static string? ToHex(ByteString bytes) =>
        bytes.Length == 0 ? null : Convert.ToHexStringLower(bytes.Span);
}
