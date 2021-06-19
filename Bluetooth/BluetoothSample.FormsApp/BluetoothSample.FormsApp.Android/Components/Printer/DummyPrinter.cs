namespace BluetoothSample.FormsApp.Droid.Components.Printer
{
    using System;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    using Android.Bluetooth;
    using Android.Content;

    using BluetoothSample.FormsApp.Components.Printer;

    public class DummyPrinter : IPrinter
    {
        private static readonly Func<BluetoothDevice, bool> Finder = x => x.Name?.Contains("DummyPrinter") ?? false;

        private static readonly byte[] Pin = Encoding.ASCII.GetBytes("8888");

        private readonly Context context;

        private readonly BluetoothAdapter adapter;

        public DummyPrinter(Context context)
        {
            this.context = context;
            adapter = ((BluetoothManager)context.GetSystemService(Context.BluetoothService)!).Adapter!;
        }

        public async ValueTask<bool> WriteAsync(string command)
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
            // TODO この方法だと処理が終わるまで確認画面が出ない？
            var tcs = new TaskCompletionSource<bool>();

            var receiver = new BondReceiver(tcs, pin);
            var filter = new IntentFilter();
            filter.AddAction(BluetoothDevice.ActionPairingRequest);
            filter.AddAction(BluetoothDevice.ActionBondStateChanged);
            filter.Priority = (int)IntentFilterPriority.HighPriority;
            context.RegisterReceiver(receiver, filter);

            // Timeout
            var cts = new CancellationTokenSource(10_000);
            cts.Token.Register(() => tcs.TrySetResult(false));

            device.CreateBond();

            var result = await tcs.Task;

            System.Diagnostics.Debug.WriteLine($"**** result=[{result}]");

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
                switch (intent!.Action)
                {
                    case BluetoothDevice.ActionPairingRequest:
                        // TODO
                        var device = (BluetoothDevice)intent.GetParcelableExtra(BluetoothDevice.ExtraDevice)!;
                        System.Diagnostics.Debug.WriteLine($"**** BluetoothDevice.ActionPairingRequest {device.Name}");
                        device.SetPin(pin);
                        InvokeAbortBroadcast();
                        break;

                    case BluetoothDevice.ActionBondStateChanged:
                        var state = intent.GetIntExtra(BluetoothDevice.ExtraBondState, BluetoothDevice.Error);
                        var previousState = intent.GetIntExtra(BluetoothDevice.ExtraPreviousBondState, BluetoothDevice.Error);
                        System.Diagnostics.Debug.WriteLine($"**** BluetoothDevice.ActionBondStateChanged {previousState} -> {state}");
                        if ((Bond)state == Bond.Bonded)
                        {
                            tcs.TrySetResult(true);
                        }
                        else if ((Bond)state == Bond.None)
                        {
                            tcs.TrySetResult(false);
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
