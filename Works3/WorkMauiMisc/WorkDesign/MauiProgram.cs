namespace WorkDesign;

using CommunityToolkit.Maui;

using Fonts;

using Microsoft.Extensions.Logging;
using Microsoft.Maui.Handlers;

using SkiaSharp.Views.Maui.Controls.Hosting;

using Syncfusion.Maui.Toolkit.Hosting;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseSkiaSharp()
            .UseMauiCommunityToolkit()
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

        builder.ConfigureMauiHandlers(handlers =>
        {
#if ANDROID
            // 既存のIndicatorViewHandlerマッピングを拡張
            IndicatorViewHandler.Mapper.AppendToMapping("HideLoadingDots", (handler, view) =>
            {
                //if (handler.PlatformView is AndroidX.ViewPager2.Widget.TabLayout tabLayout)
                //{
                //    // 初期表示を非表示に設定
                //    tabLayout.Alpha = 0;

                //    // データバインディング完了後に表示する
                //    tabLayout.Post(() =>
                //    {
                //        tabLayout.Alpha = 1;
                //    });
                //}
            });
#endif
        });

#if DEBUG
        builder.Logging.AddDebug();
#endif

#if ANDROID
        Template.MobileApp.Behaviors.EntryOption.UseCustomMapper();
#endif

        return builder.Build();
    }
}
