using Smart.Maui.Messaging;
using Smart.Mvvm.ViewModels;

namespace WorkBlazorViewModel;
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
            });

        builder.Services.AddMauiBlazorWebView();

#if DEBUG
		builder.Services.AddBlazorWebViewDeveloperTools();
		builder.Logging.AddDebug();
#endif

        builder.Services.AddSingleton<IReactiveMessenger>(ReactiveMessenger.Default);
        builder.Services.AddSingleton(BusyState.Default);

        return builder.Build();
    }
}
