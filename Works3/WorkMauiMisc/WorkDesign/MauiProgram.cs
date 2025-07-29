using SkiaSharp.Views.Maui.Controls.Hosting;

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
            .UseSkiaSharp()
            .ConfigureSyncfusionToolkit()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                //fonts.AddFont("DSEG7Classic-Regular.ttf", "DSEG7");
                SKFontFactory.AddFont(fonts, "DSEG7Classic-Regular.ttf", "DSEG7");
                //fonts.AddFont("NotoSerifJP-Medium.ttf", "NotoSerifJPMedium");
                //fonts.AddFont("NotoSerifJP-Regular.ttf", "NotoSerifJPRegular");
                SKFontFactory.AddFont(fonts, "NotoSerifJP-Medium.ttf", "NotoSerifJP-Medium");
                SKFontFactory.AddFont(fonts, "NotoSerifJP-Regular.ttf", "NotoSerifJP-Regular");
            })
            .ConfigureCustomBehaviors();

#if DEBUG
		builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
