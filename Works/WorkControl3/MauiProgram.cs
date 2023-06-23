namespace WorkControl3;

using Microsoft.Extensions.Logging;

using WorkControl3.Controls;

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
            .ConfigureCustomControls();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        // Customize only custom entry
        Microsoft.Maui.Handlers.EntryHandler.Mapper.AppendToMapping("CustomEntry", (handler, entry) =>
        {
#if ANDROID
            if (entry is CustomEntry)
            {
                handler.PlatformView.SetSelectAllOnFocus(true);
                handler.PlatformView.ShowSoftInputOnFocus = false;
            }
#endif
        });
        return builder.Build();
    }
}