#pragma warning disable IDE0130
// ReSharper disable once CheckNamespace
namespace OtelClient;

using Android.App;
using Android.Content.PM;

[Activity(
    Name = "otelsample.client.MainActivity",
    Theme = "@style/Maui.SplashTheme",
    MainLauncher = true,
    LaunchMode = LaunchMode.SingleTop,
    ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
public sealed class MainActivity : MauiAppCompatActivity;
