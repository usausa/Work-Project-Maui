namespace DeviceSample;

using System.Runtime.InteropServices;

using DeviceSample.Helpers;
using DeviceSample.Modules;

public sealed partial class App
{
    private readonly INavigator navigator;

    public App(IServiceProvider serviceProvider, ILogger<App> log)
    {
        InitializeComponent();

        navigator = serviceProvider.GetRequiredService<INavigator>();
        MainPage = serviceProvider.GetRequiredService<MainPage>();

        // Start
        log.InfoApplicationStart();
        log.InfoApplicationSettingsRuntime(RuntimeInformation.OSDescription, RuntimeInformation.FrameworkDescription, RuntimeInformation.RuntimeIdentifier);
        log.InfoApplicationSettingsEnvironment(typeof(App).Assembly.GetName().Version, Environment.CurrentDirectory);
    }

    protected override async void OnStart()
    {
        // Report previous exception
        await CrashReport.ShowReport();

        await navigator.ForwardAsync(ViewId.Menu);
    }
}
