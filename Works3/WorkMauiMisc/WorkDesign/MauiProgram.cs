namespace WorkDesign;
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
                fonts.AddFont("DSEG7Classic-Regular.ttf", "DSEG7");
            })
            .ConfigureCustomBehaviors();

#if DEBUG
		builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
