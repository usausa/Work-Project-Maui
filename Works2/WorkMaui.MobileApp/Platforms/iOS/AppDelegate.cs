// ReSharper disable once CheckNamespace
namespace WorkMaui.MobileApp;

using Foundation;

[Register("AppDelegate")]
#pragma warning disable CA1711
public class AppDelegate : MauiUIApplicationDelegate
{
    protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
}
#pragma warning restore CA1711
