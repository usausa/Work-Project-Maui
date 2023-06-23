namespace WorkControl3;

using Microsoft.Extensions.Logging;

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
            });

#if DEBUG
	builder.Logging.AddDebug();
#endif

        // Customize only custom entry
        Microsoft.Maui.Handlers.EntryHandler.Mapper.AppendToMapping("NoKeyboardEntry", (handler, entry) =>
        {
#if ANDROID
            handler.PlatformView.ShowSoftInputOnFocus = false;
#endif
        });
        return builder.Build();
    }
}