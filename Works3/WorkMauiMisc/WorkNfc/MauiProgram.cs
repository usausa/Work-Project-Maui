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
                        Debug.WriteLine("* Created 2");
                    });
                    android.OnResume(activity =>
                    {
                        Debug.WriteLine("* OnResume 2");
                        ActivityResolver.CurrentActivity = activity;
                    });
                    android.OnNewIntent((activity, intent) =>
                    {
                        Debug.WriteLine($"* OnNewIntent {intent?.Action}");
                        if (intent?.Action == NfcAdapter.ActionTechDiscovered)
                        {
                            NfcReader.ProcessIntent(intent);
                        }
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
