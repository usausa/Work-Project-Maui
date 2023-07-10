namespace WorkNativeFocus;

using Android.App;
using Android.Content.PM;
using Android.Views;

[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity
{
    public override bool DispatchKeyEvent(KeyEvent e)
    {
        System.Diagnostics.Debug.WriteLine($"{e.KeyCode} {e.Action}");

        if (e.Action == KeyEventActions.Up)
        {
            if (e.KeyCode == Keycode.F1)
            {
                EventHub.Default.Handle(null, new ForwardEventArgs { Forward = false });
            }
            if (e.KeyCode == Keycode.F2)
            {
                EventHub.Default.Handle(null, new ForwardEventArgs { Forward = true });
            }
        }

        return base.DispatchKeyEvent(e);
    }
}
