namespace WorkNativeFocus;

using Android.Views;

using Microsoft.Maui.Platform;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();

        EventHub.Default.Handle += Handle;
    }

    private void Handle(object sender, ForwardEventArgs e)
    {
        System.Diagnostics.Debug.WriteLine("*****");

        var ff = FocusFinder.Instance!;
        //System.Diagnostics.Debug.WriteLine($" ff{ff}");
        var vg = (ViewGroup)Content.ToPlatform(Content.Handler!.MauiContext!);
        //System.Diagnostics.Debug.WriteLine($"content {vg}");

        //var next = ff.FindNextFocus(vg, vg.FindFocus(), e.Forward ? FocusSearchDirection.Forward : FocusSearchDirection.Backward);
        var next = ff.FindNextFocus(vg, vg.FindFocus(), e.Forward ? FocusSearchDirection.Down : FocusSearchDirection.Up);
        System.Diagnostics.Debug.WriteLine($"next {next}");

        next?.RequestFocus();
    }
}

