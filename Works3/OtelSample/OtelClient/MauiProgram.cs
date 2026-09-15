namespace OtelClient;

using OtelClient.Services;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(static fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

#if DEBUG
        builder.Logging.AddDebug();
#endif

        // MAUI のレイアウト計測 (Meter "Microsoft.Maui") は IMeterFactory が登録されているときだけ作られる
        builder.Services.AddMetrics();

        // 送信基盤。アプリの ILogger も ILoggerProvider として登録した本クラス経由で OpenTelemetry へ流す
        builder.Services.AddSingleton<TelemetryHost>();
        builder.Services.AddSingleton<ILoggerProvider>(static p => p.GetRequiredService<TelemetryHost>());
        builder.Services.AddSingleton<MainViewModel>();
        builder.Services.AddTransient<MainPage>();

        var app = builder.Build();

        // 未処理例外の送信と、保存済みの接続先があれば起動時からの送信
        var telemetry = app.Services.GetRequiredService<TelemetryHost>();
        telemetry.RegisterCrashHandlers();
        telemetry.StartIfConfigured();

        return app;
    }
}
