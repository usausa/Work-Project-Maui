#if ANDROID
#nullable enable
namespace WorkPrinter;

using Android.Bluetooth;
using Android.Content;
using Android.Util;

using Java.Util;

public sealed class BluetoothHelper
{
    // TODO on demand
    private readonly Context context;

    private readonly BluetoothAdapter adapter;

    public BluetoothHelper(Context context)
    {
        this.context = context;
        adapter = ((BluetoothManager)context.GetSystemService(Context.BluetoothService)!).Adapter!;
    }

    public bool IsAdapterEnabled() => adapter.IsEnabled;

    public async ValueTask<BluetoothDevice?> FindDeviceAsync(Func<BluetoothDevice, bool> predicate)
    {
        var tcs = new TaskCompletionSource<BluetoothDevice?>();

        var receiver = new FindReceiver(tcs, predicate);
        var filter = new IntentFilter();
        filter.AddAction(BluetoothDevice.ActionFound);
        filter.AddAction(BluetoothAdapter.ActionDiscoveryFinished);
        context.RegisterReceiver(receiver, filter);

        if (!adapter.StartDiscovery())
        {
            context.UnregisterReceiver(receiver);
            return null;
        }

        var device = await tcs.Task;

        adapter.CancelDiscovery();

        context.UnregisterReceiver(receiver);

        return device;
    }

    public class FindReceiver : BroadcastReceiver
    {
        private readonly TaskCompletionSource<BluetoothDevice?> tcs;

        private readonly Func<BluetoothDevice, bool> predicate;

        public FindReceiver(TaskCompletionSource<BluetoothDevice?> tcs, Func<BluetoothDevice, bool> predicate)
        {
            this.tcs = tcs;
            this.predicate = predicate;
        }

        public override void OnReceive(Context? context, Intent? intent)
        {
            switch (intent!.Action)
            {
                case BluetoothDevice.ActionFound:
                    var device = intent.GetParcelableExtra<BluetoothDevice>(BluetoothDevice.ExtraDevice);
                    if (device is not null)
                    {
                        Log.Debug(nameof(BluetoothHelper), $"BluetoothDevice.ActionFound. name=[{device.Name}]");

                        if (predicate(device))
                        {
                            tcs.TrySetResult(device);
                        }
                    }
                    break;

                case BluetoothAdapter.ActionDiscoveryFinished:
                    Log.Debug(nameof(BluetoothHelper), "BluetoothDevice.ActionDiscoveryFinished.");
                    tcs.TrySetResult(null);
                    break;
            }
        }
    }

    public async ValueTask<bool> BondAsync(BluetoothDevice device, byte[] pin)
    {
        var tcs = new TaskCompletionSource<bool>();

        var receiver = new BondReceiver(tcs, pin);
        var filter = new IntentFilter();
        filter.AddAction(BluetoothDevice.ActionPairingRequest);
        filter.AddAction(BluetoothDevice.ActionBondStateChanged);
        //filter.Priority = (int)IntentFilterPriority.HighPriority;
        context.RegisterReceiver(receiver, filter);

        // Timeout
        //var cts = new CancellationTokenSource(10_000);
        //cts.Token.Register(() => tcs.TrySetResult(false));

        if (!device.CreateBond())
        {
            context.UnregisterReceiver(receiver);
            return false;
        }

        var result = await tcs.Task;

        //cts.Dispose();
        context.UnregisterReceiver(receiver);

        return result;
    }

    public class BondReceiver : BroadcastReceiver
    {
        private readonly TaskCompletionSource<bool> tcs;

        private readonly byte[] pin;

        public BondReceiver(TaskCompletionSource<bool> tcs, byte[] pin)
        {
            this.tcs = tcs;
            this.pin = pin;
        }

        public override void OnReceive(Context? context, Intent? intent)
        {
            switch (intent!.Action)
            {
                case BluetoothDevice.ActionPairingRequest:
                    var device = intent.GetParcelableExtra<BluetoothDevice>(BluetoothDevice.ExtraDevice);
                    if (device is not null)
                    {
                        Log.Debug(nameof(BluetoothHelper), $"BluetoothDevice.ActionPairingRequest. name=[{device.Name}]");

                        if (pin.Length > 0)
                        {
                            device.SetPin(pin);
                        }
                    }
                    break;

                case BluetoothDevice.ActionBondStateChanged:
                    var state = (Bond)intent.GetIntExtra(BluetoothDevice.ExtraBondState, BluetoothDevice.Error);
                    var previousState =
                        (Bond)intent.GetIntExtra(BluetoothDevice.ExtraPreviousBondState, BluetoothDevice.Error);

                    Log.Debug(nameof(BluetoothHelper), $"BluetoothDevice.ActionBondStateChanged. [{previousState}] -> [{state}]");

                    if (state == Bond.Bonded)
                    {
                        tcs.TrySetResult(true);
                    }
                    else if (state == Bond.None)
                    {
                        tcs.TrySetResult(false);
                    }
                    break;
            }
        }
    }
}

public sealed class BluetoothPrinterOption
{
    public Func<BluetoothDevice, bool> FindFilter { get; set; } = default!;

    public byte[] Pin { get; set; } = Array.Empty<byte>();
}

// TODO permission
// TODO port
// TODO primitive + extensions ?
public sealed class BluetoothSerialPort
{
    private static readonly UUID SppUuid = UUID.FromString("00001101-0000-1000-8000-00805F9B34FB")!;


}

internal class BluetoothPermission : Permissions.BasePlatformPermission
{
    public override (string androidPermission, bool isRuntime)[] RequiredPermissions => new List<(string androidPermission, bool isRuntime)>
    {
        (Android.Manifest.Permission.BluetoothScan, true),
        (Android.Manifest.Permission.BluetoothConnect, true),
        (Android.Manifest.Permission.BluetoothAdvertise, true)
    }.ToArray();
}

public static class IntentExtensions
{
    public static T? GetParcelableExtra<T>(this Intent intent, string key)
        where T : Java.Lang.Object
    {
        return OperatingSystem.IsAndroidVersionAtLeast(33)
            ? (T?)intent.GetParcelableExtra(key, Java.Lang.Class.FromType(typeof(T)))
            : (T?)intent.GetParcelableExtra(key);
    }
}
#endif
