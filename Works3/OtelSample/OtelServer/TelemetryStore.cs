namespace OtelServer;

using System.Globalization;

using OpenTelemetry.Proto.Collector.Logs.V1;
using OpenTelemetry.Proto.Collector.Metrics.V1;
using OpenTelemetry.Proto.Collector.Trace.V1;
using OpenTelemetry.Proto.Common.V1;
using OpenTelemetry.Proto.Metrics.V1;
using OpenTelemetry.Proto.Resource.V1;

// 受信した OTLP を直近 N 件だけメモリに保持する (サンプルのため永続化しない)
internal sealed class TelemetryStore
{
    private const int Capacity = 300;

    private const string ServiceNameKey = "service.name";

    private const string DeviceIdKey = "device.id";

    private readonly Lock sync = new();

    private readonly Queue<LogEntry> logs = new();

    private readonly Queue<SpanEntry> spans = new();

    private readonly Queue<MetricEntry> metrics = new();

    private readonly HashSet<string> devices = [];

    private int batches;

    private DateTimeOffset? lastReceived;

    //--------------------------------------------------------------------------------
    // Query
    //--------------------------------------------------------------------------------

    public Summary GetSummary()
    {
        lock (sync)
        {
            return new Summary(logs.Count, spans.Count, metrics.Count, batches, lastReceived, devices.Order(StringComparer.Ordinal).ToArray());
        }
    }

    // 新しいものが先頭
    public IReadOnlyList<LogEntry> GetLogs()
    {
        lock (sync)
        {
            return logs.Reverse().ToArray();
        }
    }

    public IReadOnlyList<SpanEntry> GetSpans()
    {
        lock (sync)
        {
            return spans.Reverse().ToArray();
        }
    }

    public IReadOnlyList<MetricEntry> GetMetrics()
    {
        lock (sync)
        {
            return metrics.Reverse().ToArray();
        }
    }

    //--------------------------------------------------------------------------------
    // Add (OTLP → entry)
    //--------------------------------------------------------------------------------

    public int AddLogs(ExportLogsServiceRequest request)
    {
        var count = 0;
        lock (sync)
        {
            foreach (var resourceLogs in request.ResourceLogs)
            {
                var (service, device) = ReadResource(resourceLogs.Resource);
                foreach (var scopeLogs in resourceLogs.ScopeLogs)
                {
                    foreach (var record in scopeLogs.LogRecords)
                    {
                        var attributes = ToDictionary(record.Attributes);
                        attributes["scope"] = scopeLogs.Scope?.Name ?? string.Empty;
                        Enqueue(logs, new LogEntry(
                            ToTime(record.TimeUnixNano != 0 ? record.TimeUnixNano : record.ObservedTimeUnixNano),
                            service,
                            device,
                            record.SeverityText.Length > 0 ? record.SeverityText : record.SeverityNumber.ToString(),
                            ToText(record.Body),
                            record.TraceId.Length > 0 ? Convert.ToHexStringLower(record.TraceId.Span) : null,
                            attributes));
                        count++;
                    }
                }
            }

            Touch(count);
        }

        return count;
    }

    public int AddSpans(ExportTraceServiceRequest request)
    {
        var count = 0;
        lock (sync)
        {
            foreach (var resourceSpans in request.ResourceSpans)
            {
                var (service, device) = ReadResource(resourceSpans.Resource);
                foreach (var scopeSpans in resourceSpans.ScopeSpans)
                {
                    foreach (var span in scopeSpans.Spans)
                    {
                        var attributes = ToDictionary(span.Attributes);
                        attributes["scope"] = scopeSpans.Scope?.Name ?? string.Empty;
                        Enqueue(spans, new SpanEntry(
                            ToTime(span.StartTimeUnixNano),
                            service,
                            device,
                            span.Name,
                            (span.EndTimeUnixNano - span.StartTimeUnixNano) / 1_000_000d,
                            span.Status?.Code.ToString() ?? string.Empty,
                            Convert.ToHexStringLower(span.TraceId.Span),
                            Convert.ToHexStringLower(span.SpanId.Span),
                            span.ParentSpanId.Length > 0 ? Convert.ToHexStringLower(span.ParentSpanId.Span) : null,
                            attributes));
                        count++;
                    }
                }
            }

            Touch(count);
        }

        return count;
    }

    public int AddMetrics(ExportMetricsServiceRequest request)
    {
        var count = 0;
        lock (sync)
        {
            foreach (var resourceMetrics in request.ResourceMetrics)
            {
                var (service, device) = ReadResource(resourceMetrics.Resource);
                foreach (var scopeMetrics in resourceMetrics.ScopeMetrics)
                {
                    foreach (var metric in scopeMetrics.Metrics)
                    {
                        foreach (var entry in ToEntries(metric, service, device))
                        {
                            Enqueue(metrics, entry);
                            count++;
                        }
                    }
                }
            }

            Touch(count);
        }

        return count;
    }

    //--------------------------------------------------------------------------------
    // Helper
    //--------------------------------------------------------------------------------

    private void Touch(int count)
    {
        if (count > 0)
        {
            batches++;
            lastReceived = DateTimeOffset.Now;
        }
    }

    private (string Service, string Device) ReadResource(Resource? resource)
    {
        var service = "unknown";
        var device = string.Empty;
        if (resource is not null)
        {
            foreach (var attribute in resource.Attributes)
            {
                if (attribute.Key == ServiceNameKey)
                {
                    service = ToText(attribute.Value);
                }
                else if (attribute.Key == DeviceIdKey)
                {
                    device = ToText(attribute.Value);
                }
            }
        }

        if (device.Length > 0)
        {
            devices.Add(device);
        }

        return (service, device);
    }

    private static void Enqueue<T>(Queue<T> queue, T entry)
    {
        queue.Enqueue(entry);
        while (queue.Count > Capacity)
        {
            queue.Dequeue();
        }
    }

    // メトリクスは種別ごとにデータポイントを 1 件ずつ平坦化する
    private static IEnumerable<MetricEntry> ToEntries(Metric metric, string service, string device)
    {
        switch (metric.DataCase)
        {
            case Metric.DataOneofCase.Gauge:
                foreach (var point in metric.Gauge.DataPoints)
                {
                    yield return new MetricEntry(ToTime(point.TimeUnixNano), service, device, metric.Name, metric.Unit, "gauge", ToNumber(point), ToDictionary(point.Attributes));
                }

                break;
            case Metric.DataOneofCase.Sum:
                foreach (var point in metric.Sum.DataPoints)
                {
                    yield return new MetricEntry(ToTime(point.TimeUnixNano), service, device, metric.Name, metric.Unit, metric.Sum.IsMonotonic ? "counter" : "updown", ToNumber(point), ToDictionary(point.Attributes));
                }

                break;
            case Metric.DataOneofCase.Histogram:
                foreach (var point in metric.Histogram.DataPoints)
                {
                    var value = $"count={point.Count} sum={point.Sum.ToString("0.###", CultureInfo.InvariantCulture)}" +
                                (point.HasMin ? $" min={point.Min.ToString("0.###", CultureInfo.InvariantCulture)}" : string.Empty) +
                                (point.HasMax ? $" max={point.Max.ToString("0.###", CultureInfo.InvariantCulture)}" : string.Empty);
                    yield return new MetricEntry(ToTime(point.TimeUnixNano), service, device, metric.Name, metric.Unit, "histogram", value, ToDictionary(point.Attributes));
                }

                break;
            default:
                yield return new MetricEntry(DateTimeOffset.Now, service, device, metric.Name, metric.Unit, metric.DataCase.ToString(), string.Empty, new Dictionary<string, string>(StringComparer.Ordinal));
                break;
        }
    }

    private static string ToNumber(NumberDataPoint point) =>
        point.ValueCase == NumberDataPoint.ValueOneofCase.AsInt
            ? point.AsInt.ToString(CultureInfo.InvariantCulture)
            : point.AsDouble.ToString("0.###", CultureInfo.InvariantCulture);

    private static DateTimeOffset ToTime(ulong unixNano) =>
        unixNano == 0 ? DateTimeOffset.Now : DateTimeOffset.FromUnixTimeMilliseconds((long)(unixNano / 1_000_000)).ToLocalTime();

    private static Dictionary<string, string> ToDictionary(IEnumerable<KeyValue> attributes)
    {
        var dictionary = new Dictionary<string, string>(StringComparer.Ordinal);
        foreach (var attribute in attributes)
        {
            dictionary[attribute.Key] = ToText(attribute.Value);
        }

        return dictionary;
    }

    private static string ToText(AnyValue? value) =>
        value?.ValueCase switch
        {
            AnyValue.ValueOneofCase.StringValue => value.StringValue,
            AnyValue.ValueOneofCase.BoolValue => value.BoolValue ? "true" : "false",
            AnyValue.ValueOneofCase.IntValue => value.IntValue.ToString(CultureInfo.InvariantCulture),
            AnyValue.ValueOneofCase.DoubleValue => value.DoubleValue.ToString("0.###", CultureInfo.InvariantCulture),
            AnyValue.ValueOneofCase.ArrayValue => "[" + String.Join(", ", value.ArrayValue.Values.Select(ToText)) + "]",
            AnyValue.ValueOneofCase.KvlistValue => "{" + String.Join(", ", value.KvlistValue.Values.Select(static x => $"{x.Key}={ToText(x.Value)}")) + "}",
            _ => string.Empty
        };
}
