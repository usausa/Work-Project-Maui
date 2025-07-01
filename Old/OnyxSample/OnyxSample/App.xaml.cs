namespace OnyxSample;

using OnyxSample.Helpers;
using OnyxSample.Modules;

public sealed partial class App
{
    private readonly INavigator navigator;

    public App(IServiceProvider serviceProvider, ILogger<App> log)
    {
        InitializeComponent();

        navigator = serviceProvider.GetRequiredService<INavigator>();
        MainPage = serviceProvider.GetRequiredService<MainPage>();

        // Start
        log.InfoApplicationStart(typeof(App).Assembly.GetName().Version, Environment.Version);
    }

    protected override async void OnStart()
    {
        // Report previous exception
        await CrashReport.ShowReport();

        await navigator.ForwardAsync(ViewId.Menu);
    }
}
