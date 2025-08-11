using AndroidX.Core.SplashScreen;

namespace WorkSplash;
using Android.App;
using Android.Content.PM;
using Android.OS;

using static Android.Views.ViewTreeObserver;
using static AndroidX.Core.SplashScreen.SplashScreen;

[Activity(Theme = "@style/Theme.StartLogo", MainLauncher = true, LaunchMode = LaunchMode.SingleTop, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity, IKeepOnScreenCondition, IOnExitAnimationListener
{
    private DateTime start;

    // https://shapeshifter.design/

    protected override void OnCreate(Bundle? savedInstanceState)
    {
        var splash = InstallSplashScreen(this);

        base.OnCreate(savedInstanceState);

        start = DateTime.Now;

        //splash.SetKeepOnScreenCondition(this);
        //splash.SetOnExitAnimationListener(this);
    }

    public bool ShouldKeepOnScreen()
    {
        //System.Diagnostics.Debug.WriteLine("* ShouldKeepOnScreen");
        //if (DateTime.Now - start < TimeSpan.FromSeconds(3))
        //{
        //    // Wait for 2 seconds before showing the content
        //    return false;
        //}

        return true;
    }

    public void OnSplashScreenExit(SplashScreenViewProvider splashScreenViewProvider)
    {
    }
}
