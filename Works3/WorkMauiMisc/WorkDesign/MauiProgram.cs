namespace WorkDesign;
using Microsoft.Extensions.Logging;

using Syncfusion.Maui.Toolkit.Hosting;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureSyncfusionToolkit()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                fonts.AddFont("DSEG7Classic-Regular.ttf", "DSEG7");
                fonts.AddFont("SourceHanSans-Medium.otf", "SourceHanSansMedium");
                fonts.AddFont("SourceHanSans-Regular.otf", "SourceHanSansRegular");
            })
            .ConfigureCustomBehaviors();

#if DEBUG
		builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
