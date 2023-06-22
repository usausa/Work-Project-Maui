using Microsoft.Extensions.Logging;
using WorkControl2.Behaviors;
using WorkControl2.Controls;

namespace WorkControl2
{
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

            // Apply
            TintImageMapper.ApplyTintColor();

            // Customize only custom entry
            Microsoft.Maui.Handlers.EntryHandler.Mapper.AppendToMapping("SelectEntry", (handler, entry) =>
            {
#if ANDROID
                // Custom
                if (entry is SelectEntry)
                {
                    handler.PlatformView.SetSelectAllOnFocus(true);
                }
#endif
            });

            return builder.Build();
        }
    }
}