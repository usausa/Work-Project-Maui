namespace WorkEffect;

using System.Diagnostics;

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

        Microsoft.Maui.Handlers.EntryHandler.Mapper.AppendToMapping("IsEnabled", (handler, entry) =>
        {
#if ANDROID
            Debug.WriteLine("*");
            handler.PlatformView.SetSelectAllOnFocus(true);
            handler.PlatformView.Background = null;
#endif
        });

        return builder.Build();
    }
}
