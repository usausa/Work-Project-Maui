namespace OtelServer.Telemetry.Services;

using System.Security.Cryptography;

using OtelServer.Telemetry.Models;
using OtelServer.Telemetry.Storage;

// 画面確認用のダミーデータ (OtelClient が送る内容に似せたもの) をストアへ直接投入する
public sealed class DummyDataGenerator
{
    private const string ServiceName = "OtelClient";

    private const string ScopeName = "OtelClient.MainViewModel";

    private const string HttpScopeName = "System.Net.Http";

    private static readonly (string Id, string Model, string OsVersion, string Version)[] Devices =
    [
        ("dummy-pixel-9a", "Pixel 9a", "16", "1.0.0"),
        ("dummy-pixel-8", "Pixel 8", "15", "1.0.0"),
        ("dummy-sm-s911", "SM-S911C", "14", "0.9.0")
    ];

    private static readonly (string Body, string Severity, int Number)[] LogBodies =
    [
        ("Telemetry started", "Information", 9),
        ("Button clicked", "Debug", 5),
        ("Work completed", "Information", 9),
        ("Work failed", "Error", 17),
        ("Crash button pressed", "Warning", 13),
        ("Unhandled exception.", "Critical", 21)
    ];

    private static readonly string[] Buttons = ["Info", "Warning", "Error", "Work"];

    private readonly ITelemetryStore store;

    public DummyDataGenerator(ITelemetryStore store)
    {
        this.store = store;
    }

    public int InjectMetrics(int count)
    {
        var now = DateTimeOffset.UtcNow;
        var points = new List<MetricPoint>(count * 5);
        for (var i = 0; i < count; i++)
        {
            var resource = BuildResource(Devices[Next(Devices.Length)]);
            var time = now.AddSeconds(-5 * (count - i));
            points.Add(Metric(now, resource, "app.button.clicks", "Button clicks", "1", MetricKind.Sum, time, Next(1, 50), null, null, null, [new("button", Buttons[Next(Buttons.Length)])]));
            var work = NextDouble(120, 320);
            points.Add(Metric(now, resource, "app.work.duration", "Work duration", "ms", MetricKind.Histogram, time, work, Next(1, 30), work - NextDouble(10, 60), work + NextDouble(10, 120), []));
            points.Add(Metric(now, resource, "device.battery.level", "Battery level", "%", MetricKind.Gauge, time, Next(20, 101), null, null, null, []));
            points.Add(Metric(now, resource, "dotnet.gc.heap.total_allocated", "Total allocated", "By", MetricKind.Sum, time, Next(50_000_000, 900_000_000), null, null, null, []));
            var http = NextDouble(0.01, 0.08);
            points.Add(Metric(now, resource, "http.client.request.duration", "HTTP client request duration", "s", MetricKind.Histogram, time, http, Next(1, 20), http * 0.6, http * 2.5, [new("http.request.method", "GET"), new("http.response.status_code", "200")]));
        }

        store.AddMetrics(points);
        return points.Count;
    }

    public int InjectLogs(int count)
    {
        var now = DateTimeOffset.UtcNow;
        var logs = new List<LogEntry>(count);
        for (var i = 0; i < count; i++)
        {
            logs.Add(Log(now, BuildResource(Devices[Next(Devices.Length)]), LogBodies[Next(LogBodies.Length)], now.AddSeconds(-Next(0, 600)), null, null));
        }

        store.AddLogs(logs);
        return logs.Count;
    }

    // count 件のトレース (Work → Compute + GET) と、そのトレース ID を持つログ
    public (int Spans, int Logs) InjectTraces(int count)
    {
        var now = DateTimeOffset.UtcNow;
        var spans = new List<SpanEntry>(count * 3);
        var logs = new List<LogEntry>(count);
        for (var i = 0; i < count; i++)
        {
            var resource = BuildResource(Devices[Next(Devices.Length)]);
            var traceId = Hex(16);
            var rootId = Hex(8);
            var failed = Next(5) == 0;
            var start = now.AddSeconds(-Next(0, 600));
            var compute = TimeSpan.FromMilliseconds(NextDouble(80, 200));
            var http = TimeSpan.FromMilliseconds(NextDouble(15, 60));
            var end = start + compute + http + TimeSpan.FromMilliseconds(NextDouble(5, 30));

            spans.Add(new SpanEntry(now, resource, ScopeName, traceId, rootId, null, "Work", "Internal", start, end, failed ? "ERROR" : "OK", failed ? "Work failed (sample)" : null, [new("work.kind", failed ? "fail" : "success")]));
            spans.Add(new SpanEntry(now, resource, ScopeName, traceId, Hex(8), rootId, "Compute", "Internal", start.AddMilliseconds(2), start.AddMilliseconds(2) + compute, "UNSET", null, []));
            var httpStart = start.AddMilliseconds(4) + compute;
            spans.Add(new SpanEntry(now, resource, HttpScopeName, traceId, Hex(8), rootId, "GET", "Client", httpStart, httpStart + http, "UNSET", null, [new("http.request.method", "GET"), new("url.full", "http://localhost:8080/api/time"), new("http.response.status_code", "200")]));

            logs.Add(Log(now, resource, failed ? LogBodies[3] : LogBodies[2], end, traceId, rootId));
        }

        store.AddSpans(spans);
        store.AddLogs(logs);
        return (spans.Count, logs.Count);
    }

    private static MetricPoint Metric(DateTimeOffset now, ResourceInfo resource, string name, string description, string unit, MetricKind kind, DateTimeOffset time, double value, long? count, double? min, double? max, List<KeyValueAttr> attributes) =>
        new(now, resource, ServiceName, name, description, unit, kind, time, value, count, min, max, attributes);

    private static LogEntry Log(DateTimeOffset now, ResourceInfo resource, (string Body, string Severity, int Number) body, DateTimeOffset time, string? traceId, string? spanId)
    {
        List<KeyValueAttr> attributes = body.Number >= 17
            ? [new("exception.type", "System.InvalidOperationException"), new("exception.message", body.Body + " (sample)"), new("exception.stacktrace", "   at OtelClient.MainViewModel.RunWorkAsync()\n   at OtelClient.MainViewModel.Work()")]
            : [];
        return new LogEntry(now, resource, ScopeName, time, body.Severity, body.Number, body.Body, traceId, spanId, attributes);
    }

    private static ResourceInfo BuildResource((string Id, string Model, string OsVersion, string Version) device) =>
        new(
        [
            new(ResourceInfo.ServiceNameKey, ServiceName),
            new(ResourceInfo.ServiceVersionKey, device.Version),
            new(ResourceInfo.DeviceIdKey, device.Id),
            new(ResourceInfo.DeviceModelKey, device.Model),
            new(ResourceInfo.OsNameKey, "Android"),
            new(ResourceInfo.OsVersionKey, device.OsVersion)
        ]);

    private static int Next(int max) => RandomNumberGenerator.GetInt32(max);

    private static int Next(int min, int max) => RandomNumberGenerator.GetInt32(min, max);

    private static double NextDouble(double min, double max) => min + ((max - min) * RandomNumberGenerator.GetInt32(10000) / 10000d);

    private static string Hex(int length) => Convert.ToHexStringLower(RandomNumberGenerator.GetBytes(length));
}
