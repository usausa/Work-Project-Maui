namespace WorkMaui.MobileApp;

using System.Reflection;

using CommunityToolkit.Maui;

using Smart.Resolver;

using WorkMaui.MobileApp.Modules;

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
            //.ConfigureService(services =>
            //{
            //})
            .ConfigureContainer(new SmartServiceProviderFactory(), ConfigureContainer);

#if DEBUG
        builder.Logging.AddDebug();
#endif

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

        // Components
        config.BindSingleton<IMauiInitializeService, ApplicationInitializer>();

        config.BindSingleton<ApplicationState>();

        config.AddNavigator(c =>
        {
            c.UseMauiNavigationProvider();
            // TODO
            //c.AddPlugin<NavigationFocusPlugin>();
            c.UseIdViewMapper(m =>
                m.AutoRegister(Assembly.GetExecutingAssembly().UnderNamespaceTypes(typeof(ViewId))));
        });
    }
}