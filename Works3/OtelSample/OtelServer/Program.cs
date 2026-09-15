using Google.Protobuf;

using OpenTelemetry.Proto.Collector.Logs.V1;
using OpenTelemetry.Proto.Collector.Metrics.V1;
using OpenTelemetry.Proto.Collector.Trace.V1;

using OtelServer;

var builder = WebApplication.CreateBuilder(args);

// OTLP/HTTP の本文は gzip されることがある
builder.Services.AddRequestDecompression();
builder.Services.AddSingleton(new TelemetryStore());

var app = builder.Build();

app.UseRequestDecompression();
app.UseDefaultFiles();
app.UseStaticFiles();

// OTLP/HTTP (protobuf) の受け口。OpenTelemetry の OTLP エクスポーター (HttpProtobuf) が POST してくる
app.MapPost("/v1/logs", static async (HttpRequest request, TelemetryStore store, ILogger<Program> logger) =>
{
    var payload = await ReadAsync<ExportLogsServiceRequest>(request, ExportLogsServiceRequest.Parser, logger, "logs").ConfigureAwait(false);
    if (payload is null)
    {
        return Results.BadRequest();
    }

    var count = store.AddLogs(payload);
    var service = ServiceOf(payload.ResourceLogs.Select(static x => x.Resource));
    logger.InfoReceived("logs", service, count);
    return Results.Bytes(new ExportLogsServiceResponse().ToByteArray(), "application/x-protobuf");
});

app.MapPost("/v1/traces", static async (HttpRequest request, TelemetryStore store, ILogger<Program> logger) =>
{
    var payload = await ReadAsync<ExportTraceServiceRequest>(request, ExportTraceServiceRequest.Parser, logger, "traces").ConfigureAwait(false);
    if (payload is null)
    {
        return Results.BadRequest();
    }

    var count = store.AddSpans(payload);
    var service = ServiceOf(payload.ResourceSpans.Select(static x => x.Resource));
    logger.InfoReceived("traces", service, count);
    return Results.Bytes(new ExportTraceServiceResponse().ToByteArray(), "application/x-protobuf");
});

app.MapPost("/v1/metrics", static async (HttpRequest request, TelemetryStore store, ILogger<Program> logger) =>
{
    var payload = await ReadAsync<ExportMetricsServiceRequest>(request, ExportMetricsServiceRequest.Parser, logger, "metrics").ConfigureAwait(false);
    if (payload is null)
    {
        return Results.BadRequest();
    }

    var count = store.AddMetrics(payload);
    var service = ServiceOf(payload.ResourceMetrics.Select(static x => x.Resource));
    logger.InfoReceived("metrics", service, count);
    return Results.Bytes(new ExportMetricsServiceResponse().ToByteArray(), "application/x-protobuf");
});

// 画面 (wwwroot/index.html) が読む JSON
app.MapGet("/api/summary", static (TelemetryStore store) => Results.Ok(store.GetSummary()));
app.MapGet("/api/logs", static (TelemetryStore store) => Results.Ok(store.GetLogs()));
app.MapGet("/api/spans", static (TelemetryStore store) => Results.Ok(store.GetSpans()));
app.MapGet("/api/metrics", static (TelemetryStore store) => Results.Ok(store.GetMetrics()));

// クライアントの HttpClient 計装 (クライアント側スパン) の確認用
app.MapGet("/api/time", static () => Results.Ok(new { Time = DateTimeOffset.Now }));

app.Run();

// 本文 (protobuf) を読む。壊れていれば null
static async Task<T?> ReadAsync<T>(HttpRequest request, MessageParser<T> parser, ILogger logger, string signal)
    where T : class, IMessage<T>
{
    try
    {
        using var stream = new MemoryStream();
        await request.Body.CopyToAsync(stream).ConfigureAwait(false);
        return parser.ParseFrom(stream.ToArray());
    }
    catch (InvalidProtocolBufferException ex)
    {
        logger.WarnInvalidPayload(signal, ex);
        return null;
    }
}

static string ServiceOf(IEnumerable<OpenTelemetry.Proto.Resource.V1.Resource?> resources) =>
    resources.FirstOrDefault()?.Attributes.FirstOrDefault(static x => x.Key == "service.name")?.Value.StringValue ?? "unknown";
