using Plugin.Maui.DebugRainbows;

namespace WorkDebug;
using Microsoft.Extensions.Logging;

using Plugin.Maui.DebugOverlay;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        // https://github.com/davidortinau/Plugin.Maui.DebugOverlay
        // https://github.com/sthewissen/Plugin.Maui.DebugRainbows
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseDebugOverlay()
            //.UseDebugRibbon(Colors.Orange)
#if DEBUG
            //.UseDebugRainbows()
            .UseDebugRainbows(new DebugRainbowsOptions
            {
                ShowRainbows = true,
                ShowGrid = true,
                HorizontalItemSize = 20,
                VerticalItemSize = 20,
                MajorGridLineInterval = 4,
                MajorGridLines = new GridLineOptions { Color = Color.FromRgb(255, 0, 0), Opacity = 0.5, Width = 3 },
                MinorGridLines = new GridLineOptions { Color = Color.FromRgb(255, 0, 0), Opacity = 0.25, Width = 1 },
                GridOrigin = DebugGridOrigin.TopLeft,
            })
#endif
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

#if DEBUG
		builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
