namespace OnyxSample;

using CommunityToolkit.Maui;

using MauiComponents.Resolver;

using Microsoft.Maui.LifecycleEvents;

using OnyxSample.Behaviors;

using Smart.Resolver;

using OnyxSample.Components.Storage;
using OnyxSample.Helpers;
using OnyxSample.Modules;
using OnyxSample.Usecase;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        // Builder
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
#if ANDROID
            // ReSharper disable UnusedParameter.Local
            .ConfigureLifecycleEvents(events =>
            {
                // Lifecycle
#if DEVICE_FULL_SCREEN
                events.AddAndroid(android => android.OnCreate(static (activity, _) => AndroidHelper.FullScreen(activity)));
#endif
            })
            // ReSharper restore UnusedParameter.Local
#endif
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                fonts.AddFont("Roboto-Regular.ttf", "RobotoRegular");
                fonts.AddFont("MaterialIcons-Regular.ttf", "MaterialIcons");
                fonts.AddFont("Font Awesome 6 Free-Regular-400.otf", "FontAwesome");
            })
            //.ConfigureEssentials(static c => { })
            .UseMauiCommunityToolkit()
            .UseMauiCommunityToolkitCamera()
            .UseMauiInterfaces()
            .UseCommunityToolkitInterfaces()
            .ConfigureCustomBehaviors()
            .ConfigureContainer(new SmartServiceProviderFactory(), ConfigureContainer);

        // Logging
        builder.Logging
#if DEBUG
            .AddDebug()
#endif
#if ANDROID && !DEBUG
            .AddAndroidLogger(static options => options.ShortCategory = true)
#endif
            .AddFileLogger(static options =>
            {
#if ANDROID
                options.Directory = Path.Combine(AndroidHelper.GetExternalFilesDir(), "log");
#endif
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
#if ANDROID
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
#endif
        config.AddComponentsSerializer();

        // Navigator
        config.AddNavigator(c =>
        {
            c.UseMauiNavigationProvider();
            c.AddResolverPlugin();
            c.UseIdViewMapper(static m => m.AutoRegister(ViewRegistry.ViewSource()));
        });

        // Components
        config.BindSingleton<IStorageManager, StorageManager>();

        // State
        config.BindSingleton<ApplicationState>();
        config.BindSingleton<Session>();
        config.BindSingleton<Settings>();

        // Usecase
        config.BindSingleton<CognitiveUsecase>();

        // Startup
        config.BindSingleton<IMauiInitializeService, ApplicationInitializer>();
    }
}
