namespace Template.MobileApp;

using Microsoft.Extensions.DependencyInjection;

using Template.MobileApp.Helpers;
using Template.MobileApp.Modules;

public sealed partial class App
{
    private readonly IServiceProvider serviceProvider;

    public App(IServiceProvider serviceProvider, ILogger<App> log)
    {
        this.serviceProvider = serviceProvider;

        // Light theme based application
        Current!.UserAppTheme = AppTheme.Light;

        InitializeComponent();

        // Start
        log.InfoApplicationStart(typeof(App).Assembly.GetName().Version, Environment.Version);
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        return new Window(serviceProvider.GetRequiredService<MainPage>());
    }

    // ReSharper disable once AsyncVoidMethod
    // 例外時はグローバルハンドラ経由でCrashReportに記録されfail-fastとなる (次回起動時に表示)
    protected override async void OnStart()
    {
        // Report previous exception
        await CrashReport.ShowReport();

        // 権限要求は起動時に一括では行わず、各機能の利用画面側でCheck→Requestする

        // 非同期初期化(DB再構築)の完了を待ってから画面遷移する
        await serviceProvider.GetRequiredService<ApplicationInitializer>().StartupTask;

        // Navigate
        var navigator = serviceProvider.GetRequiredService<INavigator>();
        await navigator.ForwardAsync(ViewId.Menu);
    }
}
