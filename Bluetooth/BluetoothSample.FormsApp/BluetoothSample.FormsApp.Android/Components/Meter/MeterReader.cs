namespace BluetoothSample.FormsApp.Droid.Components.Meter
{
    using System;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    using Android.Bluetooth;
    using Android.Content;

    using BluetoothSample.FormsApp.Components.Meter;

    public class MeterReader : IMeterReader
    {
        private static readonly Func<BluetoothDevice, bool> Finder = x => x.Name?.StartsWith("BTWATTCH2") ?? false;

        private static readonly byte[] Pin = Encoding.ASCII.GetBytes("0000");

        private readonly Context context;

        private readonly BluetoothAdapter adapter;

        public MeterReader(Context context)
        {
            this.context = context;
            adapter = ((BluetoothManager)context.GetSystemService(Context.BluetoothService)!).Adapter!;
        }

        public async ValueTask<bool> ReadAsync()
        {
            // Find
            var device = await FindBluetoothDeviceAsync(Finder);
            if (device is null)
            {
                return false;
            }

            // Bond
            if ((device.BondState != Bond.Bonded) &&
                !await BondAsync(device, Pin))
            {
                return false;
            }

            // TODO

            return true;
        }

        private async ValueTask<bool> BondAsync(BluetoothDevice device, byte[] pin)
        {
            var tcs = new TaskCompletionSource<bool>();

            var receiver = new BondReceiver(tcs, pin);
            var filter = new IntentFilter();
            filter.AddAction(BluetoothDevice.ActionPairingRequest);
            filter.AddAction(BluetoothDevice.ActionBondStateChanged);
            //filter.Priority = (int)IntentFilterPriority.HighPriority - 1;
            context.RegisterReceiver(receiver, filter);

            // Timeout
            var cts = new CancellationTokenSource(10_000);
            cts.Token.Register(() => tcs.TrySetResult(false));

            device.SetPin(pin);
            device.CreateBond();

            var result = await tcs.Task;

            cts.Dispose();
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
                System.Diagnostics.Debug.WriteLine($"**** {intent!.Action}");
                switch (intent!.Action)
                {
                    // TODO not happen ?
                    case BluetoothDevice.ActionPairingRequest:
                        var device = (BluetoothDevice)intent.GetParcelableExtra(BluetoothDevice.ExtraDevice)!;
                        device.SetPin(pin);
                        InvokeAbortBroadcast();
                        System.Diagnostics.Debug.WriteLine("**** BluetoothDevice.ActionPairingRequest");
                        break;

                    case BluetoothDevice.ActionBondStateChanged:
                        // TODO 11 to 10 false ?
                        var state = intent.GetIntExtra(BluetoothDevice.ExtraBondState, BluetoothDevice.Error);
                        //var previousState = intent.GetIntExtra(BluetoothDevice.ExtraPreviousBondState, BluetoothDevice.Error);
                        System.Diagnostics.Debug.WriteLine($"**** BluetoothDevice.ActionPairingRequest {state}");
                        if ((Bond)state == Bond.Bonded)
                        {
                            tcs.TrySetResult(true);
                        }
                        break;
                }
            }
        }

        private async ValueTask<BluetoothDevice?> FindBluetoothDeviceAsync(Func<BluetoothDevice, bool> predicate)
        {
            var tcs = new TaskCompletionSource<BluetoothDevice?>();

            var receiver = new FindReceiver(tcs, predicate);
            var filter = new IntentFilter();
            filter.AddAction(BluetoothDevice.ActionFound);
            filter.AddAction(BluetoothAdapter.ActionDiscoveryFinished);
            context.RegisterReceiver(receiver, filter);

            adapter.StartDiscovery();

            var device = await tcs.Task;

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
                        var device = (BluetoothDevice)intent.GetParcelableExtra(BluetoothDevice.ExtraDevice)!;
                        if (predicate(device))
                        {
                            tcs.TrySetResult(device);
                        }
                        break;

                    case BluetoothAdapter.ActionDiscoveryFinished:
                        tcs.TrySetResult(null);
                        break;
                }
            }
        }
    }
}
