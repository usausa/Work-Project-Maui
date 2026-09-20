namespace OtelServer.Telemetry.Services;

using OtelServer.Telemetry.Storage;

// 保持期間を過ぎた行を 1 時間ごとに削除する
public sealed class TelemetryRetentionService : BackgroundService
{
    private static readonly TimeSpan Interval = TimeSpan.FromHours(1);

    private readonly ILogger<TelemetryRetentionService> logger;

    private readonly TelemetryStoreOptions options;

    private readonly ITelemetryStore store;

    public TelemetryRetentionService(ILogger<TelemetryRetentionService> logger, TelemetryStoreOptions options, ITelemetryStore store)
    {
        this.logger = logger;
        this.options = options;
        this.store = store;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (options.RetentionDays <= 0)
        {
            logger.InfoRetentionDisabled();
            return;
        }

        using var timer = new PeriodicTimer(Interval);
        try
        {
            do
            {
                var deleted = store.PurgeExpired();
                if (deleted > 0)
                {
                    var retentionDays = options.RetentionDays;
                    logger.InfoTelemetryPurged(deleted, retentionDays);
                }
            }
            while (await timer.WaitForNextTickAsync(stoppingToken).ConfigureAwait(false));
        }
        catch (OperationCanceledException)
        {
            // 停止
        }
    }
}
