namespace OtelServer.Telemetry.Storage;

using OtelServer.Telemetry.Models;

public interface ITelemetryStore
{
    event EventHandler<EventArgs>? Changed;

    void AddMetrics(IEnumerable<MetricPoint> points);

    void AddLogs(IEnumerable<LogEntry> logs);

    void AddSpans(IEnumerable<SpanEntry> spans);

    IReadOnlyList<string> GetServiceNames();

    IReadOnlyList<string> GetDeviceIds();

    IReadOnlyList<DeviceInfo> GetDevices();

    IReadOnlyList<string> GetMetricNames(string serviceName);

    MetricSeriesSnapshot? GetMetricSeries(string serviceName, string metricName, int maxPoints = 500);

    IReadOnlyList<LogEntry> GetLogs(LogQuery query);

    IReadOnlyList<TraceSummary> GetTraces(TraceQuery query);

    TraceDetail? GetTrace(string traceId);

    IngestHistory GetIngestHistory(TimeSpan window);

    TelemetrySummary GetSummary();

    void Clear();

    int PurgeExpired();
}
