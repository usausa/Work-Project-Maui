namespace OtelClient.Services;

using System.Diagnostics;
using System.Diagnostics.Metrics;

using Microsoft.Extensions.DependencyInjection;

using OpenTelemetry;
using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

// OpenTelemetry の送信基盤。トレース / メトリクス / ログを OTLP/HTTP (protobuf) で OtelServer へ送る
// - 接続先は Preferences に保存し、適用のたびにプロバイダーを作り直す (Stop → Start)
// - アプリの ILogger は ILoggerProvider として登録した本クラス経由で、送信中だけ OpenTelemetry へ流す
// - 送信の失敗は例外にならず SDK の EventSource に出るので SdkEventListener で拾う
// - 送信に失敗したバッチはディスクへ退避し 60 秒ごとに再送する (OTEL_DOTNET_EXPERIMENTAL_OTLP_RETRY=disk)
// - 未処理例外はログ (Critical) として送り、落ちる前に送信する
// - Stop / Flush は溜まっている分の送信を待つ (接続先が落ちていると数秒) ため UI スレッドから直接呼ばない
public sealed partial class TelemetryHost : ILoggerProvider
{
    public const string ServiceName = "OtelClient";

    // MAUI 自身の計測 (.NET 10: レイアウトの measure / arrange)。メトリクスは常に、スパンは設定で送る
    private const string MauiSourceName = "Microsoft.Maui";

    // HttpClient のメトリクス (.NET 8 以降の組み込み。スパンは AddHttpClientInstrumentation)
    private const string HttpMeterName = "System.Net.Http";

    private static readonly ActivitySource Source = new(ServiceName);

    private static readonly Meter Meter = new(ServiceName);

    private static readonly HttpClient HttpClient = new();

    private readonly Counter<long> clickCounter = Meter.CreateCounter<long>("app.button.clicks", "{click}", "ボタンの押下回数");

    private readonly Histogram<double> workDuration = Meter.CreateHistogram<double>("app.work.duration", "ms", "処理の所要時間");

    private readonly SdkEventListener listener = new();

    private readonly Lock sync = new();

    private readonly ILogger logger;

    private readonly string retryDirectory;

    private TracerProvider? tracerProvider;

    private MeterProvider? meterProvider;

    // ログの ILoggerFactory / LoggerProvider を閉じ込めた DI コンテナ
    private ServiceProvider? logServices;

    private int crashReported;

    private bool disposed;

    public event EventHandler? StateChanged;

    public event EventHandler<SdkEventArgs>? SdkEventWritten;

    public string DeviceId { get; }

    public bool IsRunning => Options is not null;

    public TelemetryOptions? Options { get; private set; }

    public TelemetryHost()
    {
        logger = new ForwardingLogger(this, nameof(TelemetryHost));
        DeviceId = TelemetrySettings.GetDeviceId();
        retryDirectory = Path.Combine(FileSystem.Current.CacheDirectory, "otlp");

        // 電池残量は観測時に読む
        Meter.CreateObservableGauge("device.battery.level", static () => Battery.Default.ChargeLevel * 100, "%", "電池残量");

        listener.Written += OnSdkEventWritten;
    }

    public void Dispose()
    {
        if (disposed)
        {
            return;
        }

        disposed = true;
        Stop();
        listener.Written -= OnSdkEventWritten;
        listener.Dispose();
    }

    // ILoggerProvider: アプリの ILogger を送信中の OpenTelemetry へ転送する
    public ILogger CreateLogger(string categoryName) => new ForwardingLogger(this, categoryName);

    //--------------------------------------------------------------------------------
    // Lifecycle
    //--------------------------------------------------------------------------------

    // 保存済みの設定があれば送信を始める
    public void StartIfConfigured()
    {
        var endpoint = Preferences.Default.Get(TelemetrySettings.EndpointKey, string.Empty);
        if (Uri.TryCreate(endpoint, UriKind.Absolute, out var uri))
        {
            Start(new TelemetryOptions(uri, Preferences.Default.Get(TelemetrySettings.MauiSpansKey, false)));
        }
    }

    // 設定を保存してプロバイダーを作り直す
    public void Start(TelemetryOptions options)
    {
        lock (sync)
        {
            StopCore();
            StartCore(options);
        }

        logger.InfoTelemetryStarted(options.Endpoint);
        StateChanged?.Invoke(this, EventArgs.Empty);
    }

    public void Stop()
    {
        lock (sync)
        {
            if (tracerProvider is null)
            {
                return;
            }

            StopCore();
        }

        StateChanged?.Invoke(this, EventArgs.Empty);
    }

    // 溜まっている分を今すぐ送る
    public void Flush(int timeoutMilliseconds = 3000)
    {
        lock (sync)
        {
            tracerProvider?.ForceFlush(timeoutMilliseconds);
            meterProvider?.ForceFlush(timeoutMilliseconds);
            logServices?.GetRequiredService<LoggerProvider>().ForceFlush(timeoutMilliseconds);
        }
    }

    // ディスクへ退避され再送を待っているバッチの数
    public int CountPendingFiles() =>
        Directory.Exists(retryDirectory)
            ? Directory.EnumerateFiles(retryDirectory, "*.blob", SearchOption.AllDirectories).Count()
            : 0;

    private void StartCore(TelemetryOptions options)
    {
        // 送信に失敗したバッチをディスクへ退避して再送する (実験的機能。設定は環境変数で読まれる)
        Environment.SetEnvironmentVariable("OTEL_DOTNET_EXPERIMENTAL_OTLP_RETRY", "disk");
        Environment.SetEnvironmentVariable("OTEL_DOTNET_EXPERIMENTAL_OTLP_DISK_RETRY_DIRECTORY_PATH", retryDirectory);

        var resource = ResourceBuilder.CreateDefault()
            .AddService(ServiceName, serviceVersion: AppInfo.Current.VersionString)
            .AddAttributes(new Dictionary<string, object>(StringComparer.Ordinal)
            {
                ["device.id"] = DeviceId,
                ["device.model"] = $"{DeviceInfo.Current.Manufacturer} {DeviceInfo.Current.Model}",
                ["os.name"] = DeviceInfo.Current.Platform.ToString(),
                ["os.version"] = DeviceInfo.Current.VersionString
            });

        var tracerBuilder = Sdk.CreateTracerProviderBuilder()
            .SetResourceBuilder(resource)
            .AddSource(ServiceName)
            .AddHttpClientInstrumentation()
            .AddOtlpExporter(exporter =>
            {
                Configure(exporter, options.Endpoint, "v1/traces");
                exporter.BatchExportProcessorOptions.ScheduledDelayMilliseconds = 2000;
            });
        if (options.IncludeMauiSpans)
        {
            tracerBuilder.AddSource(MauiSourceName);
        }

        tracerProvider = tracerBuilder.Build();

        meterProvider = Sdk.CreateMeterProviderBuilder()
            .SetResourceBuilder(resource)
            .AddMeter(ServiceName)
            .AddMeter(MauiSourceName)
            .AddMeter(HttpMeterName)
            .AddRuntimeInstrumentation()
            // MAUI のレイアウト計測は要素ごと (element.id / element.frame) にタグが付き系列が際限なく増えるため型だけで集計する
            .AddView("maui.layout.*", new MetricStreamConfiguration { TagKeys = ["element.type"] })
            .AddOtlpExporter((exporter, reader) =>
            {
                Configure(exporter, options.Endpoint, "v1/metrics");
                reader.PeriodicExportingMetricReaderOptions.ExportIntervalMilliseconds = 5000;
            })
            .Build();

        // ログは ILoggerFactory 経由でしか作れないため専用の DI コンテナに閉じ込める (Flush のために LoggerProvider も取り出す)
        var services = new ServiceCollection();
        services.AddLogging(logging => logging
            .SetMinimumLevel(LogLevel.Information)
            .AddOpenTelemetry(otel =>
            {
                otel.SetResourceBuilder(resource);
                otel.IncludeFormattedMessage = true;
                otel.IncludeScopes = true;
                otel.AddOtlpExporter((exporter, processor) =>
                {
                    Configure(exporter, options.Endpoint, "v1/logs");
                    processor.BatchExportProcessorOptions.ScheduledDelayMilliseconds = 2000;
                });
            }));
        logServices = services.BuildServiceProvider();

        Options = options;
        Preferences.Default.Set(TelemetrySettings.EndpointKey, options.Endpoint.ToString());
        Preferences.Default.Set(TelemetrySettings.MauiSpansKey, options.IncludeMauiSpans);
    }

    private void StopCore()
    {
        // Dispose は溜まっている分を送ってから閉じる (プロバイダーごとに最大 5 秒)
        logServices?.Dispose();
        logServices = null;
        meterProvider?.Dispose();
        meterProvider = null;
        tracerProvider?.Dispose();
        tracerProvider = null;

        Options = null;
    }

    //--------------------------------------------------------------------------------
    // Signals
    //--------------------------------------------------------------------------------

    public void CountClick(string kind) =>
        clickCounter.Add(1, new KeyValuePair<string, object?>("kind", kind));

    // 親スパン + 子スパン (計算) + HttpClient のスパン (計装) をつくる。fail のときはエラーを記録する
    // ReSharper disable ExplicitCallerInfoArgument (スパン名は呼び出し元のメソッド名ではなく明示する)
    public async Task<WorkResult> RunWorkAsync(bool fail, CancellationToken cancellationToken = default)
    {
        using var activity = Source.StartActivity("Work");
        activity?.SetTag("work.fail", fail);
        var stopwatch = Stopwatch.StartNew();
        try
        {
            using (var child = Source.StartActivity("Compute"))
            {
                await Task.Delay(120, cancellationToken).ConfigureAwait(false);
                child?.SetTag("compute.items", 42);
            }

            var time = await FetchServerTimeAsync(cancellationToken).ConfigureAwait(false);
            if (fail)
            {
                throw new InvalidOperationException("Work failed (sample).");
            }

            activity?.SetStatus(ActivityStatusCode.Ok);
            logger.InfoWorkCompleted(time);
            return new WorkResult(true, time);
        }
        catch (Exception ex) when (ex is InvalidOperationException or HttpRequestException or TaskCanceledException)
        {
            activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
            activity?.AddException(ex);
            logger.ErrorWorkFailed(ex);
            return new WorkResult(false, ex.Message);
        }
        finally
        {
            workDuration.Record(stopwatch.Elapsed.TotalMilliseconds, new KeyValuePair<string, object?>("work.fail", fail));
        }
    }

    // ReSharper restore ExplicitCallerInfoArgument

    //--------------------------------------------------------------------------------
    // Crash
    //--------------------------------------------------------------------------------

    // 未処理例外のフックを登録する
    public void RegisterCrashHandlers()
    {
        AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
        TaskScheduler.UnobservedTaskException += OnUnobservedTaskException;
        PlatformRegisterCrashHandler();
    }

    // 未処理例外をログとして送り、落ちる前に送信する (2 回目以降は無視)
    public void ReportCrash(Exception exception)
    {
        if (Interlocked.Exchange(ref crashReported, 1) != 0)
        {
            return;
        }

        logger.CriticalUnhandled(exception);
        Flush();
    }

    partial void PlatformRegisterCrashHandler();

    private void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        if (e.ExceptionObject is Exception exception)
        {
            ReportCrash(exception);
        }
    }

    private void OnUnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs e)
    {
        logger.ErrorUnobservedTask(e.Exception);
    }

    //--------------------------------------------------------------------------------
    // Helper
    //--------------------------------------------------------------------------------

    private static void Configure(OtlpExporterOptions options, Uri endpoint, string path)
    {
        // OTLP/HTTP。エンドポイントはシグナルごとのパスまで指定する
        options.Protocol = OtlpExportProtocol.HttpProtobuf;
        options.Endpoint = new Uri(endpoint, path);
        options.TimeoutMilliseconds = 10000;
    }

    private async Task<string> FetchServerTimeAsync(CancellationToken cancellationToken)
    {
        if (Options is null)
        {
            return string.Empty;
        }

        using var response = await HttpClient.GetAsync(new Uri(Options.Endpoint, "api/time"), cancellationToken).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
    }

    private void OnSdkEventWritten(object? sender, SdkEventArgs e)
    {
        SdkEventWritten?.Invoke(this, e);
    }

    // 送信中のロガー。停止中 (コンテナ無し) や停止と同時のときは null
    private ILogger? CreateInnerLogger(string category)
    {
        // ReSharper disable once InconsistentlySynchronizedField (参照を読むだけ。停止と同時のときは ObjectDisposedException を握る)
        var services = logServices;
        if (services is null)
        {
            return null;
        }

        try
        {
            return services.GetRequiredService<ILoggerFactory>().CreateLogger(category);
        }
        catch (ObjectDisposedException)
        {
            return null;
        }
    }

    // 送信中の OpenTelemetry のロガーへ転送する。停止中は何もしない
    private sealed class ForwardingLogger : ILogger
    {
        private readonly TelemetryHost host;

        private readonly string category;

        public ForwardingLogger(TelemetryHost host, string category)
        {
            this.host = host;
            this.category = category;
        }

        public IDisposable? BeginScope<TState>(TState state)
            where TState : notnull =>
            host.CreateInnerLogger(category)?.BeginScope(state);

        public bool IsEnabled(LogLevel logLevel) =>
            host.IsRunning && (logLevel != LogLevel.None) && (logLevel >= LogLevel.Information);

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter) =>
            host.CreateInnerLogger(category)?.Log(logLevel, eventId, state, exception, formatter);
    }
}
