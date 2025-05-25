using MauiIcons.Core;
using MauiIcons.Fluent;
using MauiIcons.Fluent.Filled;
using MauiIcons.FontAwesome;
using MauiIcons.FontAwesome.Brand;
using MauiIcons.FontAwesome.Solid;
using MauiIcons.Material;
using MauiIcons.Material.Outlined;
using MauiIcons.Material.Rounded;
using MauiIcons.Material.Sharp;

using Microsoft.Extensions.Logging;

namespace WorkIcon
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiIconsCore(x =>
                {
                    x.SetDefaultIconSize(30.0);
                    x.SetDefaultIconAutoScaling(true);
                    x.SetDefaultFontOverride(true);
                })
                .UseMaterialMauiIcons()
                .UseMaterialOutlinedMauiIcons()
                .UseMaterialRoundedMauiIcons()
                .UseMaterialSharpMauiIcons()
                //.UseSegoeFluentMauiIcons()
                .UseFluentMauiIcons()
                .UseFluentFilledMauiIcons()
                //.UseCupertinoMauiIcons()
                .UseFontAwesomeMauiIcons()
                .UseFontAwesomeSolidMauiIcons()
                .UseFontAwesomeBrandMauiIcons()
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
}
