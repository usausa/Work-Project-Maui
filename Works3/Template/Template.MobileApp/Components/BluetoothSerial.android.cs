namespace Template.MobileApp.Components;

using Android.Bluetooth;
using Android.Content;
using Android.Util;

public sealed partial class BluetoothSerialFactory
{
    // TODO
#pragma warning disable CA1822
    public partial ValueTask<IBluetoothSerial?> ConnectAsync(string name, string? pin)
    {
        return ValueTask.FromResult((IBluetoothSerial?)null);
    }
#pragma warning restore CA1822

    // TODO delete & merge to BluetoothSerialFactory
    // ReSharper disable once UnusedType.Local
    private sealed class BluetoothHelper
    {
        private readonly Context context;

        private readonly BluetoothAdapter adapter;

        public BluetoothHelper(Context context)
        {
            this.context = context;
            adapter = ((BluetoothManager)context.GetSystemService(Context.BluetoothService)!).Adapter!;
        }

        private async ValueTask<BluetoothDevice?> FindDeviceAsync(Func<BluetoothDevice, bool> predicate)
        {
            var tcs = new TaskCompletionSource<BluetoothDevice?>();

            using var receiver = new FindReceiver(tcs, predicate);
            using var filter = new IntentFilter();
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

        private sealed class FindReceiver : BroadcastReceiver
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
#pragma warning disable CA1422
                        var device = (BluetoothDevice)intent.GetParcelableExtra(BluetoothDevice.ExtraDevice)!;
#pragma warning restore CA1422
                        Log.Debug(nameof(BluetoothHelper), $"[BluetoothDevice.ActionFound] {device.Name}");
                        if (predicate(device))
                        {
                            tcs.TrySetResult(device);
                        }
                        break;

                    case BluetoothAdapter.ActionDiscoveryFinished:
                        Log.Debug(nameof(BluetoothHelper), "[BluetoothAdapter.ActionDiscoveryFinished]");
                        tcs.TrySetResult(null);
                        break;
                }
            }
        }

        public async ValueTask<bool> BondAsync(BluetoothDevice device, byte[] pin)
        {
            var tcs = new TaskCompletionSource<bool>();

            using var receiver = new BondReceiver(tcs, pin);
            using var filter = new IntentFilter();
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

        private sealed class BondReceiver : BroadcastReceiver
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
#pragma warning disable CA1422
                        var device = (BluetoothDevice)intent.GetParcelableExtra(BluetoothDevice.ExtraDevice)!;
#pragma warning restore CA1422
                        Log.Debug(nameof(BluetoothHelper), $"[BluetoothDevice.ActionPairingRequest] {device.Name}");
                        if (pin.Length > 0)
                        {
                            device.SetPin(pin);
                        }
                        break;

                    case BluetoothDevice.ActionBondStateChanged:
                        var state = (Bond)intent.GetIntExtra(BluetoothDevice.ExtraBondState, BluetoothDevice.Error);
                        var previousState = (Bond)intent.GetIntExtra(BluetoothDevice.ExtraPreviousBondState, BluetoothDevice.Error);
                        Log.Debug(nameof(BluetoothHelper), $"[BluetoothDevice.ActionBondStateChanged] {previousState} -> {state}");
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

    // ------------------------------------------------------------
    // BluetoothSerial
    // ------------------------------------------------------------

    // TODO delete
    // ReSharper disable once UnusedType.Local
    private sealed class BluetoothSerial : IBluetoothSerial
    {
        private readonly BluetoothSocket socket;

        public BluetoothSerial(BluetoothSocket socket)
        {
            this.socket = socket;
        }

        public void Dispose()
        {
            socket.Close();
        }

        public async ValueTask WriteAsync(byte[] buffer, int offset, int count)
        {
#pragma warning disable CA1835
            await socket.OutputStream!.WriteAsync(buffer, offset, count);
#pragma warning restore CA1835
        }

        public async ValueTask<int> ReadAsync(byte[] buffer, int offset, int count)
        {
#pragma warning disable CA1835
            return await socket.InputStream!.ReadAsync(buffer, offset, count);
#pragma warning restore CA1835
        }
    }
}
