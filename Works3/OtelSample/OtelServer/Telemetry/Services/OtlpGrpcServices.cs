namespace OtelServer.Telemetry.Services;

using Grpc.Core;

using OpenTelemetry.Proto.Collector.Logs.V1;
using OpenTelemetry.Proto.Collector.Metrics.V1;
using OpenTelemetry.Proto.Collector.Trace.V1;

using OtelServer.Telemetry.Storage;

// OTLP/gRPC の受け口 (HTTP/2 の平文。HTTP/1.1 と同居できないため別ポート)。中身は OTLP/HTTP と同じ
public sealed class OtlpLogsService : LogsService.LogsServiceBase
{
    private readonly ITelemetryStore store;

    private readonly ILogger<OtlpLogsService> logger;

    public OtlpLogsService(ITelemetryStore store, ILogger<OtlpLogsService> logger)
    {
        this.store = store;
        this.logger = logger;
    }

    public override Task<ExportLogsServiceResponse> Export(ExportLogsServiceRequest request, ServerCallContext context)
    {
        var logs = TelemetryMapper.MapLogs(request.ResourceLogs, DateTimeOffset.UtcNow).ToList();
        store.AddLogs(logs);
        var service = logs.FirstOrDefault()?.Resource.ServiceName ?? "unknown";
        var count = logs.Count;
        logger.InfoReceived("logs (gRPC)", service, count);
        return Task.FromResult(new ExportLogsServiceResponse());
    }
}

public sealed class OtlpTraceService : TraceService.TraceServiceBase
{
    private readonly ITelemetryStore store;

    private readonly ILogger<OtlpTraceService> logger;

    public OtlpTraceService(ITelemetryStore store, ILogger<OtlpTraceService> logger)
    {
        this.store = store;
        this.logger = logger;
    }

    public override Task<ExportTraceServiceResponse> Export(ExportTraceServiceRequest request, ServerCallContext context)
    {
        var spans = TelemetryMapper.MapSpans(request.ResourceSpans, DateTimeOffset.UtcNow).ToList();
        store.AddSpans(spans);
        var service = spans.FirstOrDefault()?.Resource.ServiceName ?? "unknown";
        var count = spans.Count;
        logger.InfoReceived("traces (gRPC)", service, count);
        return Task.FromResult(new ExportTraceServiceResponse());
    }
}

public sealed class OtlpMetricsService : MetricsService.MetricsServiceBase
{
    private readonly ITelemetryStore store;

    private readonly ILogger<OtlpMetricsService> logger;

    public OtlpMetricsService(ITelemetryStore store, ILogger<OtlpMetricsService> logger)
    {
        this.store = store;
        this.logger = logger;
    }

    public override Task<ExportMetricsServiceResponse> Export(ExportMetricsServiceRequest request, ServerCallContext context)
    {
        var points = TelemetryMapper.MapMetrics(request.ResourceMetrics, DateTimeOffset.UtcNow).ToList();
        store.AddMetrics(points);
        var service = points.FirstOrDefault()?.Resource.ServiceName ?? "unknown";
        var count = points.Count;
        logger.InfoReceived("metrics (gRPC)", service, count);
        return Task.FromResult(new ExportMetricsServiceResponse());
    }
}
