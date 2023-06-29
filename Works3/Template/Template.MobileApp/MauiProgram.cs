namespace Template.MobileApp;

using System.Reflection;

using CommunityToolkit.Maui;

using Smart.Resolver;

using Template.MobileApp.Behaviors;
using Template.MobileApp.Components.Device;
using Template.MobileApp.Controls;
using Template.MobileApp.Modules;
using Template.MobileApp.Services;
using Template.MobileApp.State;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        // Builder
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            })
            .UseMauiCommunityToolkit()
            .ConfigureCustomControls()
            .ConfigureCustomBehaviors()
            .ConfigureService(services =>
            {
#if ANDROID
                services.AddComponentsDialog();
#endif
                // TODO SourceGenerator?
                services.AddComponentsPopup(c =>
                    c.AutoRegister(Assembly.GetExecutingAssembly().UnderNamespaceTypes(typeof(DialogId))));
                services.AddComponentsSerializer();
            })
            .ConfigureContainer(new SmartServiceProviderFactory(), ConfigureContainer);

        // Logging
        builder.Logging
#if DEBUG
            .AddDebug()
#endif
#if ANDROID
            .AddAndroidLogger(options =>
            {
                options.ShortCategory = true;
            })
#endif
            .AddFileLogger(options =>
            {
#if ANDROID
                options.Directory = Path.Combine(Android.App.Application.Context.GetExternalFilesDir(string.Empty)!.Path, "log");
#endif
                options.RetainDays = 7;
            })
            .AddFilter(typeof(MauiProgram).Namespace, LogLevel.Debug);

        if (!String.IsNullOrEmpty(Variants.AppCenterSecret()))
        {
            Microsoft.AppCenter.AppCenter.Start(
                Variants.AppCenterSecret(),
                typeof(Microsoft.AppCenter.Analytics.Analytics),
                typeof(Microsoft.AppCenter.Crashes.Crashes));
        }

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

        // MAUI
        config.BindSingleton(Preferences.Default);

        // Components
        config.BindSingleton<IMauiInitializeService, ApplicationInitializer>();

        config.BindSingleton<IDeviceManager, DeviceManager>();

        // State
        config.BindSingleton<ApplicationState>();

        config.BindSingleton<Settings>();
        config.BindSingleton<Session>();

        // Service
        config.BindSingleton(new DataServiceOptions
        {
            Path = Path.Combine(FileSystem.AppDataDirectory, "Data.db")
        });

        config.BindSingleton<DataService>();
        config.AddNavigator(c =>
        {
            c.UseMauiNavigationProvider();
            // TODO
            //c.AddPlugin<NavigationFocusPlugin>();
            // TODO SourceGenerator?
            c.UseIdViewMapper(m =>
                m.AutoRegister(Assembly.GetExecutingAssembly().UnderNamespaceTypes(typeof(ViewId))));
        });
    }
}
