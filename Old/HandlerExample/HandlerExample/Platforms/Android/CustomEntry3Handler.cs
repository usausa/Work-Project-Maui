namespace HandlerExample.Platforms.Android;

using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;

using EditText = global::Android.Widget.EditText;

public class CustomEntry3Handler : ViewHandler<ICustomEntry, EditText>
{
    // PropertyMapper (OnElementChanged replacement?)
    public static PropertyMapper<ICustomEntry, CustomEntry3Handler> CustomEntryMapper = new(ViewMapper)
    {
        [nameof(ICustomEntry.Text)] = MapText,
        [nameof(ICustomEntry.TextColor)] = MapTextColor,
    };

    public CustomEntry3Handler()
        : base(CustomEntryMapper)
    {
    }

    protected override EditText CreatePlatformView()
    {
        return new EditText(Context);
    }

    private static void MapText(CustomEntry3Handler handler, ICustomEntry entry)
    {
        handler.PlatformView.Text = entry.Text;
    }

    private static void MapTextColor(CustomEntry3Handler handler, ICustomEntry entry)
    {
        handler.PlatformView.SetTextColor(entry.TextColor.ToPlatform());
    }

    protected override void ConnectHandler(EditText platformView)
    {
        System.Diagnostics.Debug.WriteLine("**** ConnectHandler()");
        base.ConnectHandler(platformView);
        // Event add
    }

    protected override void DisconnectHandler(EditText platformView)
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