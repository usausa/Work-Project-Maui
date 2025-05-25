#pragma warning disable IDE0130
// ReSharper disable once CheckNamespace
namespace DeviceSample;

using Android.App;
using Android.Content.PM;
using Android.OS;

[Activity(
    Theme = "@style/Maui.SplashTheme",
    MainLauncher = true,
    AlwaysRetainTaskState = true,
    LaunchMode = LaunchMode.SingleInstance,
    ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density,
    ScreenOrientation = ScreenOrientation.Portrait)]
public class MainActivity : MauiAppCompatActivity
{
    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);

        ActivityResolver.Init(this);
    }
}
