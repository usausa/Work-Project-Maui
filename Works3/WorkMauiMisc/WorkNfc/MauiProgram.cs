namespace WorkNfc;

using Android.Nfc;

using Microsoft.Extensions.Logging;
using Microsoft.Maui.LifecycleEvents;

using System.Diagnostics;

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
            })
            .ConfigureLifecycleEvents(events =>
            {
#if ANDROID
                events.AddAndroid(android => android.OnCreate((activity, bundle) =>
                {
                    Debug.WriteLine("* Created 1");
                }));
#endif
            })
            .ConfigureLifecycleEvents(events =>
            {
#if ANDROID
                events.AddAndroid(android =>
                {
                    android.OnCreate((activity, bundle) =>
                    {
                        ActivityResolver.CurrentActivity = activity;
                    });
                    android.OnResume(activity =>
                    {
                        ActivityResolver.CurrentActivity = activity;
                    });
                });
#endif
            });

#if DEBUG
		builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
