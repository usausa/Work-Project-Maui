namespace OtelServer.Telemetry.Storage;

// appsettings.json の TelemetryStore セクション
public sealed class TelemetryStoreOptions
{
    public string DatabasePath { get; set; } = "App_Data/otel.db";

    // この日数より古い行を 1 時間ごとに削除する (0 で無効)
    public int RetentionDays { get; set; } = 7;

    // 受信のたびに適用する件数の上限
    public int MaxPointsPerMetricSeries { get; set; } = 500;

    public int MaxLogsPerService { get; set; } = 2000;

    public int MaxTraces { get; set; } = 500;

    public int MaxSpansPerTrace { get; set; } = 200;
}
