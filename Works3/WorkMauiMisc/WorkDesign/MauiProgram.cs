namespace WorkDesign;

using Fonts;

using SkiaSharp.Views.Maui.Controls.Hosting;

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
                fonts.AddFont("MaterialIcons-Regular.ttf", MaterialIcons.FontFamily);
                fonts.AddFont("DSEG7Classic-Regular.ttf", "DSEG7");
                fonts.AddFont("851Gkktt_005.ttf", "Gkktt");
                fonts.AddFont("ipaexm.ttf", "IPAexMincho");
                fonts.AddFont("NotoSerifJP-Regular.ttf", "NotoSerifJPRegular");
                fonts.AddFont("NotoSerifJP-Medium.ttf", "NotoSerifJPMedium");
                fonts.AddFont("Michroma.ttf", "Michroma");
                fonts.AddFont("Orbitron-Regular.ttf", "OrbitronRegular");
                fonts.AddFont("Orbitron-Medium.ttf", "OrbitronMedium");
                fonts.AddFont("Oxanium-Regular.ttf", "OxaniumRegular");
                fonts.AddFont("Oxanium-Medium.ttf", "OxaniumMedium");
            })
            .ConfigureCustomBehaviors();

#if DEBUG
		builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
