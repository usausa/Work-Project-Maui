namespace OtelClient;

using System.Diagnostics;
using System.Text;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using OtelClient.Services;

// 送信の開始 / 停止と、ログ・スパン・メトリクスを発生させる操作を提供する
public sealed partial class MainViewModel : ObservableObject
{
    private const int MaxEntries = 30;

    private readonly TelemetryHost telemetry;

    private readonly ILogger<MainViewModel> logger;

    private readonly IDispatcherTimer? timer;

    private readonly List<string> entries = [];

    private int sdkErrors;

    [ObservableProperty]
    public partial string Endpoint { get; set; } = Preferences.Default.Get(TelemetrySettings.EndpointKey, TelemetrySettings.DefaultEndpoint);

    [ObservableProperty]
    public partial bool UseGrpc { get; set; } = Preferences.Default.Get(TelemetrySettings.GrpcKey, false);

    [ObservableProperty]
    public partial bool IncludeMauiSpans { get; set; } = Preferences.Default.Get(TelemetrySettings.MauiSpansKey, false);

    [ObservableProperty]
    public partial string Message { get; set; } = "こんにちは";

    [ObservableProperty]
    public partial string Status { get; set; } = "停止";

    [ObservableProperty]
    public partial int PendingFiles { get; set; }

    [ObservableProperty]
    public partial string SdkMessage { get; set; } = string.Empty;

    [ObservableProperty]
    public partial bool HasSdkMessage { get; set; }

    // 操作の記録 (新しいものが上)
    [ObservableProperty]
    public partial string LogText { get; set; } = string.Empty;

    public string DeviceId => telemetry.DeviceId;

    public MainViewModel(TelemetryHost telemetry, ILogger<MainViewModel> logger)
    {
        this.telemetry = telemetry;
        this.logger = logger;

        // 送信基盤のイベントはバックグラウンドスレッドから来る
        telemetry.StateChanged += (_, _) => MainThread.BeginInvokeOnMainThread(UpdateStatus);
        telemetry.SdkEventWritten += (_, e) => MainThread.BeginInvokeOnMainThread(() => ShowSdkMessage(e));
        UpdateStatus();

        // 再送待ちの数は定期的に読む
        timer = Dispatcher.GetForCurrentThread()?.CreateTimer();
        if (timer is not null)
        {
            timer.Interval = TimeSpan.FromSeconds(2);
            timer.Tick += (_, _) => PendingFiles = telemetry.CountPendingFiles();
            timer.Start();
        }
    }

    //--------------------------------------------------------------------------------
    // Lifecycle
    //--------------------------------------------------------------------------------

    // 開始 / 停止 / Flush は溜まっている分の送信を待つためワーカーで行う
    [RelayCommand]
    private async Task ApplyAsync()
    {
        if (!Uri.TryCreate(Endpoint.Trim(), UriKind.Absolute, out var uri) ||
            ((uri.Scheme != Uri.UriSchemeHttp) && (uri.Scheme != Uri.UriSchemeHttps)))
        {
            AddEntry("ERR", "接続先の URL が不正です");
            return;
        }

        // シグナルごとのパス (v1/logs など) を後ろに付けるため末尾は / にする
        if (!uri.AbsolutePath.EndsWith('/'))
        {
            uri = new Uri(uri.AbsoluteUri + "/");
        }

        var options = new TelemetryOptions(uri, UseGrpc, IncludeMauiSpans);
        var elapsed = await RunAsync(() => telemetry.Start(options)).ConfigureAwait(true);
        AddEntry("APP", $"送信開始 {uri} {(UseGrpc ? "gRPC" : "HTTP")} ({elapsed}ms)");
    }

    [RelayCommand]
    private async Task StopAsync()
    {
        var elapsed = await RunAsync(telemetry.Stop).ConfigureAwait(true);
        AddEntry("APP", $"送信停止 ({elapsed}ms)");
    }

    [RelayCommand]
    private async Task FlushAsync()
    {
        var elapsed = await RunAsync(() => telemetry.Flush()).ConfigureAwait(true);
        AddEntry("APP", $"Flush ({elapsed}ms)");
    }

    //--------------------------------------------------------------------------------
    // Logs
    //--------------------------------------------------------------------------------

    [RelayCommand]
    private void LogInformation()
    {
        telemetry.CountClick("info");
        logger.InfoMessage(Message);
        AddEntry("LOG", $"Information: {Message}");
    }

    [RelayCommand]
    private void LogWarning()
    {
        telemetry.CountClick("warning");
        logger.WarnMessage(Message);
        AddEntry("LOG", $"Warning: {Message}");
    }

    // 例外付きのエラーログ (exception.* 属性にスタックトレースが入る)
    [RelayCommand]
    private void LogError()
    {
        telemetry.CountClick("error");
        try
        {
            throw new InvalidOperationException(Message);
        }
        catch (InvalidOperationException ex)
        {
            logger.ErrorMessage(Message, ex);
        }

        AddEntry("LOG", $"Error: {Message}");
    }

    //--------------------------------------------------------------------------------
    // Spans / Metrics / Crash
    //--------------------------------------------------------------------------------

    [RelayCommand]
    private Task RunWorkAsync() => RunWorkAsync(false);

    [RelayCommand]
    private Task RunWorkFailAsync() => RunWorkAsync(true);

    // 画面の操作でそのまま落ちる。未処理例外がログとして届くことを確かめる
    [RelayCommand]
    private void Crash()
    {
        telemetry.CountClick("crash");
        logger.WarnMessage("Crash button pressed.");
        throw new InvalidOperationException("Crash test (sample).");
    }

    //--------------------------------------------------------------------------------
    // Helper
    //--------------------------------------------------------------------------------

    private static async Task<long> RunAsync(Action action)
    {
        var stopwatch = Stopwatch.StartNew();
        await Task.Run(action).ConfigureAwait(true);
        return stopwatch.ElapsedMilliseconds;
    }

    private async Task RunWorkAsync(bool fail)
    {
        telemetry.CountClick(fail ? "work-fail" : "work");
        var stopwatch = Stopwatch.StartNew();
        var result = await telemetry.RunWorkAsync(fail).ConfigureAwait(true);
        AddEntry("SPAN", $"Work {(result.Succeeded ? "成功" : "失敗")} {stopwatch.ElapsedMilliseconds}ms {result.Message}");
    }

    // 切替時、接続先のポートが既定 (8080 / 4317) ならもう一方へ入れ替える
    partial void OnUseGrpcChanged(bool value)
    {
        var (from, to) = value ? (TelemetrySettings.HttpPort, TelemetrySettings.GrpcPort) : (TelemetrySettings.GrpcPort, TelemetrySettings.HttpPort);
        if (Uri.TryCreate(Endpoint.Trim(), UriKind.Absolute, out var uri) && (uri.Port == from))
        {
            Endpoint = new UriBuilder(uri) { Port = to }.Uri.ToString();
        }
    }

    private void UpdateStatus()
    {
        Status = telemetry.Options is { } options ? $"送信中 {options.Endpoint} ({(options.UseGrpc ? "gRPC" : "HTTP")})" : "停止";
        PendingFiles = telemetry.CountPendingFiles();
    }

    // SDK の自己診断は接続先が落ちていると数秒ごとに出るため、最新だけ件数付きで見せる
    private void ShowSdkMessage(SdkEventArgs e)
    {
        sdkErrors++;
        SdkMessage = $"{e.Level} ({sdkErrors}) {e.Source}: {e.Message}";
        HasSdkMessage = true;
    }

    private void AddEntry(string kind, string text)
    {
        entries.Insert(0, $"{DateTime.Now:HH:mm:ss} [{kind}] {text}");
        if (entries.Count > MaxEntries)
        {
            entries.RemoveAt(entries.Count - 1);
        }

        var builder = new StringBuilder();
        foreach (var entry in entries)
        {
            builder.AppendLine(entry);
        }

        LogText = builder.ToString();
    }
}
