using System.Diagnostics;
using Microsoft.Extensions.Logging;

using Other;
using WorkLog.Log;

namespace WorkLog
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

            builder.Logging
#if DEBUG
                .AddDebug()
#endif
                .AddAndroidLogger(options =>
                {
                    options.ShortCategory = true;
                })
                .AddFileLogger(options =>
                {
#if ANDROID
                    options.Directory = Path.Combine(Android.App.Application.Context.GetExternalFilesDir("")!.Path, "log");
#endif
                    options.Prefix = "app";
                })
                .AddFilter("WorkLog", LogLevel.Debug)
                .AddFilter("Other", LogLevel.Warning);

            builder.Services.AddSingleton<SingletonService>();
            builder.Services.AddTransient<TransientService>();
            builder.Services.AddSingleton<OtherService>();

            return builder.Build();
        }
    }
}