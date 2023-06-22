using System.Diagnostics;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;

namespace WorkControl.Controls.Handlers;

public partial class SimpleViewHandler : ViewHandler<ISimpleView, Android.Views.View>
{

    protected override Android.Views.View CreatePlatformView()
    {
        return new Android.Views.View(Context);
    }

    protected override void ConnectHandler(Android.Views.View platformView)
    {
        base.ConnectHandler(platformView);
        platformView.Click += PlatformViewOnClick;
    }

    protected override void DisconnectHandler(Android.Views.View platformView)
    {
        platformView.Click -= PlatformViewOnClick;
        base.DisconnectHandler(platformView);
    }

    private void PlatformViewOnClick(object sender, EventArgs e)
    {
        VirtualView.PerformClick();
    }

    private static void MapColor(SimpleViewHandler handler, ISimpleView view)
    {
        handler.PlatformView.SetBackgroundColor(view.Color.ToPlatform());
    }

    private static void MapPlatformCallRequested(SimpleViewHandler handler, ISimpleView view, object args)
    {
        Debug.WriteLine($"Platform call requested. args=[{args}]");
    }

}
