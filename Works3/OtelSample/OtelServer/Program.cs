using Google.Protobuf;

using MudBlazor.Services;

using OpenTelemetry.Proto.Collector.Logs.V1;
using OpenTelemetry.Proto.Collector.Metrics.V1;
using OpenTelemetry.Proto.Collector.Trace.V1;

using OtelServer;
using OtelServer.Components;
using OtelServer.Telemetry.Models;
using OtelServer.Telemetry.Services;
using OtelServer.Telemetry.Storage;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMudServices();
builder.Services.AddRazorComponents().AddInteractiveServerComponents();
builder.Services.AddGrpc();

// OTLP/HTTP の本文は gzip されることがある
builder.Services.AddRequestDecompression();

var storeOptions = builder.Configuration.GetSection("TelemetryStore").Get<TelemetryStoreOptions>() ?? new TelemetryStoreOptions();
builder.Services.AddSingleton(storeOptions);
builder.Services.AddSingleton<ITelemetryStore, SqliteTelemetryStore>();
builder.Services.AddSingleton<DummyDataGenerator>();
builder.Services.AddHostedService<TelemetryRetentionService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/error", createScopeForErrors: true);
}

app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseRequestDecompression();
app.UseAntiforgery();
app.MapStaticAssets();

// OTLP/gRPC の受け口 (appsettings.json の Kestrel:Endpoints:Grpc = 4317、HTTP/2 のみ)
app.MapGrpcService<OtlpLogsService>();
app.MapGrpcService<OtlpTraceService>();
app.MapGrpcService<OtlpMetricsService>();

// OTLP/HTTP (protobuf) の受け口。OpenTelemetry の OTLP エクスポーター (HttpProtobuf) が POST してくる
app.MapPost("/v1/logs", static async (HttpRequest request, ITelemetryStore store, ILogger<Program> logger) =>
{
    var payload = await ReadAsync<ExportLogsServiceRequest>(request, ExportLogsServiceRequest.Parser, logger, "logs").ConfigureAwait(false);
    if (payload is null)
    {
        return Results.BadRequest();
    }

    var logs = TelemetryMapper.MapLogs(payload.ResourceLogs, DateTimeOffset.UtcNow).ToList();
    store.AddLogs(logs);
    var service = ServiceOf(logs.Select(static x => x.Resource));
    var count = logs.Count;
    logger.InfoReceived("logs", service, count);
    return Results.Bytes(new ExportLogsServiceResponse().ToByteArray(), "application/x-protobuf");
});

app.MapPost("/v1/traces", static async (HttpRequest request, ITelemetryStore store, ILogger<Program> logger) =>
{
    var payload = await ReadAsync<ExportTraceServiceRequest>(request, ExportTraceServiceRequest.Parser, logger, "traces").ConfigureAwait(false);
    if (payload is null)
    {
        return Results.BadRequest();
    }

    var spans = TelemetryMapper.MapSpans(payload.ResourceSpans, DateTimeOffset.UtcNow).ToList();
    store.AddSpans(spans);
    var service = ServiceOf(spans.Select(static x => x.Resource));
    var count = spans.Count;
    logger.InfoReceived("traces", service, count);
    return Results.Bytes(new ExportTraceServiceResponse().ToByteArray(), "application/x-protobuf");
});

app.MapPost("/v1/metrics", static async (HttpRequest request, ITelemetryStore store, ILogger<Program> logger) =>
{
    var payload = await ReadAsync<ExportMetricsServiceRequest>(request, ExportMetricsServiceRequest.Parser, logger, "metrics").ConfigureAwait(false);
    if (payload is null)
    {
        return Results.BadRequest();
    }

    var points = TelemetryMapper.MapMetrics(payload.ResourceMetrics, DateTimeOffset.UtcNow).ToList();
    store.AddMetrics(points);
    var service = ServiceOf(points.Select(static x => x.Resource));
    var count = points.Count;
    logger.InfoReceived("metrics", service, count);
    return Results.Bytes(new ExportMetricsServiceResponse().ToByteArray(), "application/x-protobuf");
});

// JSON (curl での確認用)
app.MapGet("/api/summary", static (ITelemetryStore store) => Results.Ok(store.GetSummary()));
app.MapGet("/api/devices", static (ITelemetryStore store) => Results.Ok(store.GetDevices()));
app.MapGet("/api/logs", static (ITelemetryStore store, string? service, string? device, int? severity, string? text, string? trace, int? count) =>
    Results.Ok(store.GetLogs(new LogQuery(service, device, severity ?? 0, text, trace, count ?? 100))));
app.MapGet("/api/traces", static (ITelemetryStore store, string? service, string? device, bool? errors, string? text, int? count) =>
    Results.Ok(store.GetTraces(new TraceQuery(service, device, errors ?? false, text, count ?? 100))));
app.MapGet("/api/traces/{traceId}", static (ITelemetryStore store, string traceId) =>
    store.GetTrace(traceId) is { } trace ? Results.Ok(trace) : Results.NotFound());
app.MapGet("/api/metrics", static (ITelemetryStore store, string? service) =>
    Results.Ok(service is null ? store.GetServiceNames() : store.GetMetricNames(service)));
app.MapGet("/api/metrics/{name}", static (ITelemetryStore store, string name, string service, int? count) =>
    store.GetMetricSeries(service, name, count ?? 100) is { } series ? Results.Ok(series) : Results.NotFound());

// クライアントの HttpClient 計装 (クライアント側スパン) の確認用
app.MapGet("/api/time", static () => Results.Ok(new { Time = DateTimeOffset.Now }));

app.MapGet("/", static context =>
{
    context.Response.Redirect("/dashboard");
    return Task.CompletedTask;
});

app.MapRazorComponents<App>().AddInteractiveServerRenderMode();

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

static string ServiceOf(IEnumerable<ResourceInfo> resources) =>
    resources.FirstOrDefault()?.ServiceName ?? "unknown";
