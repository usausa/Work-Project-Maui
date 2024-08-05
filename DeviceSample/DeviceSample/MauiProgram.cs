namespace DeviceSample;

using CommunityToolkit.Maui;

using DeviceSample.Components.Storage;
using DeviceSample.Helpers;
using DeviceSample.Modules;

using MauiComponents.Resolver;

using Microsoft.Extensions.Logging;

using Plugin.Maui.Audio;

using Smart.Resolver;

using ZXing.Net.Maui.Controls;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            })
            .UseMauiCommunityToolkit()
            .UseBarcodeReader()
            .UseMauiInterfaces()
            .UseCommunityToolkitInterfaces()
            .ConfigureContainer(new SmartServiceProviderFactory(), ConfigureContainer);

        // Logging
        builder.Logging
#if DEBUG
            .AddDebug()
#endif
            .AddAndroidLogger(static options => options.ShortCategory = true)
            .AddFileLogger(static options =>
            {
                options.Directory = Path.Combine(AndroidHelper.GetExternalFilesDir(), "log");
                options.RetainDays = 7;
            })
            .AddFilter(typeof(MauiProgram).Namespace, LogLevel.Debug);

        // Crash dump
        CrashReport.Start();

        return builder.Build();
    }

    private static void ConfigureContainer(ResolverConfig config)
    {
        config
            .UseAutoBinding()
            .UseArrayBinding()
            .UseAssignableBinding()
            .UsePropertyInjector()
            .UsePageContextScope();

        // MauiComponents
        config.AddComponentsDialog(c =>
        {
            var resources = Application.Current!.Resources;
            c.IndicatorColor = resources.FindResource<Color>("BlueAccent2");
            c.LoadingMessageBackgroundColor = Colors.White;
            c.LoadingMessageColor = Colors.Black;
            c.LoadingMessageFontSize = 28;
            c.ProgressValueColor = Colors.Black;
            c.ProgressAreaBackgroundColor = Colors.White;
            c.ProgressCircleColor1 = resources.FindResource<Color>("BlueAccent2");
            c.ProgressCircleColor2 = resources.FindResource<Color>("GrayLighten2");
            c.EnablePromptEnterAction = true;
            c.EnablePromptSelectAll = true;
        });
        //config.AddComponentsPopup(static c => c.AutoRegister(ViewRegistry.DialogSource()));
        config.AddComponentsSerializer();
        config.AddComponentsScreen();
        config.AddComponentsSpeech();
        config.AddComponentsLocation();

        // Navigator
        config.AddNavigator(c =>
        {
            c.UseMauiNavigationProvider();
            c.AddResolverPlugin();
            c.UseIdViewMapper(static m => m.AutoRegister(ViewRegistry.ViewSource()));
        });

        // Components
        config.BindSingleton<IStorageManager, StorageManager>();

        config.BindSingleton(AudioManager.Current);

        // State
        config.BindSingleton<ApplicationState>();

        // Startup
        config.BindSingleton<IMauiInitializeService, ApplicationInitializer>();
    }
}
