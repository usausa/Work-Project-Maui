namespace WorkPrinter;

using System.Text;

using Android.Bluetooth;
using Android.Util;

using Java.Util;

using MauiComponents;

using Microsoft.Maui.Controls;

public partial class MainPage
{
    private static readonly UUID SppUuid = UUID.FromString("00001101-0000-1000-8000-00805F9B34FB")!;

    public MainPage()
    {
        InitializeComponent();
    }

    private async void Button_OnClicked(object sender, EventArgs e)
    {
        var result = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
        if (result != PermissionStatus.Granted)
        {
            return;
        }

#if ANDROID
        result = await Permissions.RequestAsync<BluetoothPermission>();
        if (result != PermissionStatus.Granted)
        {
            return;
        }

        var helper = new BluetoothHelper(ActivityResolver.CurrentActivity);

        //var enable = helper.IsAdapterEnabled();

        var device = await helper.FindDeviceAsync(x => x.Name is not null && x.Name.Contains("DummyPrinter"));
        if (device is null)
        {
            return;
        }

        if ((device.BondState != Bond.Bonded) && !await helper.BondAsync(device, Encoding.ASCII.GetBytes("8888")))
        {
            device = null;
            return;
        }

        // TODO socket
        // Execute
        var socket = default(BluetoothSocket);
        try
        {
            // TODO Timeout?
            socket = device.CreateRfcommSocketToServiceRecord(SppUuid)!;

            await socket.ConnectAsync();

            // Write
            var send = Encoding.ASCII.GetBytes("test");
            await socket.OutputStream!.WriteAsync(send, 0, send.Length);

            // TODO timeout, buffer ?
            // Read
            var receive = new byte[16];
            var read = await socket.InputStream!.ReadAsync(receive, 0, receive.Length);
            if (read <= 0)
            {
                device = null;
                return;
            }

            var response = Encoding.ASCII.GetString(receive, 0, read);
            System.Diagnostics.Debug.WriteLine(response);
        }
        catch (Java.IO.IOException ex)
        {
            Log.Error("DummyPrinter", ex, "Connection error.");
            device = null;
            return;
        }
        finally
        {
            socket?.Close();
        }
#endif
    }
}
