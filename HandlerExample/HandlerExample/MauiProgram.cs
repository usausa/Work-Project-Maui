namespace HandlerExample;

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
            .ConfigureMauiHandlers(config =>
            {
#if ANDROID
                config.AddHandler<CustomEntry1, HandlerExample.Platforms.Android.CustomEntry1Handler>();
                config.AddHandler<CustomEntry3, HandlerExample.Platforms.Android.CustomEntry3Handler>();
#endif
            });

        return builder.Build();
    }
}
