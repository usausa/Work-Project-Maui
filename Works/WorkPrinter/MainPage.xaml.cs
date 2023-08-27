namespace WorkPrinter;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
    }

    private async void Button_OnClicked(object sender, EventArgs e)
    {
#if ANDROID
        var result = await Permissions.CheckStatusAsync<BluetoothConnectPermission>();
        if (result != PermissionStatus.Granted)
        {
            result = await Permissions.RequestAsync<BluetoothConnectPermission>();
            if (result != PermissionStatus.Granted)
            {
                return;
            }
        }
#endif
    }
}

#if ANDROID
internal class BluetoothConnectPermission : Permissions.BasePlatformPermission
{
    public override (string androidPermission, bool isRuntime)[] RequiredPermissions => new List<(string androidPermission, bool isRuntime)>
    {
        (Android.Manifest.Permission.BluetoothScan, true),
        (Android.Manifest.Permission.BluetoothConnect, true)
    }.ToArray();
}
#endif
