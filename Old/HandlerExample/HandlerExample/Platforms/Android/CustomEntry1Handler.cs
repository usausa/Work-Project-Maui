namespace HandlerExample.Platforms.Android;

using AndroidX.AppCompat.Widget;

using Microsoft.Maui.Handlers;

public class CustomEntry1Handler : EntryHandler
{
    protected override AppCompatEditText CreatePlatformView()
    {
        var nativeView = base.CreatePlatformView();
        nativeView.SetTextColor(global::Android.Graphics.Color.White);
        nativeView.SetBackgroundColor(global::Android.Graphics.Color.Red);
        return nativeView;
    }

    protected override void ConnectHandler(AppCompatEditText platformView)
    {
        System.Diagnostics.Debug.WriteLine("**** ConnectHandler()");
        base.ConnectHandler(platformView);
        // Event add
    }

    protected override void DisconnectHandler(AppCompatEditText platformView)
    {
        System.Diagnostics.Debug.WriteLine("**** DisconnectHandler()");
        // Event remove
        base.DisconnectHandler(platformView);
    }

    protected override void SetupContainer()
    {
        System.Diagnostics.Debug.WriteLine("**** SetupContainer()");
        base.SetupContainer();
    }

    protected override void RemoveContainer()
    {
        System.Diagnostics.Debug.WriteLine("**** RemoveContainer()");
        base.RemoveContainer();
    }
}