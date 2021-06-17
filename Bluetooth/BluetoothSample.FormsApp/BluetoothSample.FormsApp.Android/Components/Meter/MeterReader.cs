namespace BluetoothSample.FormsApp.Droid.Components.Meter
{
    using System.Diagnostics;
    using System.Threading;
    using System.Threading.Tasks;

    using Android.Bluetooth;
    using Android.Content;

    using BluetoothSample.FormsApp.Components.Meter;

    public class MeterReader : IMeterReader
    {
        private readonly Context context;

        private readonly BluetoothAdapter adapter;

        public MeterReader(Context context)
        {
            this.context = context;
            adapter = ((BluetoothManager)context.GetSystemService(Context.BluetoothService)!).Adapter!;
        }

        public async ValueTask<bool> Discover()
        {
            var tcs = new TaskCompletionSource<bool>();

            var receiver = new TestReceiver(tcs);
            var filter = new IntentFilter();
            filter.AddAction(BluetoothDevice.ActionFound);
            filter.AddAction(BluetoothAdapter.ActionDiscoveryFinished);
            context.RegisterReceiver(receiver, filter);

            if (!adapter.StartDiscovery())
            {
                context.UnregisterReceiver(receiver);
                return false;
            }

            var cts = new CancellationTokenSource(60_000);
            cts.Token.Register(() => tcs.SetResult(false));

            var result = await tcs.Task;

            cts.Dispose();

            // TODO

            context.UnregisterReceiver(receiver);

            return result;
        }

        public class TestReceiver : BroadcastReceiver
        {
            private readonly TaskCompletionSource<bool> tcs;

            public TestReceiver(TaskCompletionSource<bool> tcs)
            {
                this.tcs = tcs;
            }

            public override void OnReceive(Context? context, Intent? intent)
            {
                switch (intent!.Action)
                {
                    case BluetoothDevice.ActionFound:
                        var device = (BluetoothDevice)intent.GetParcelableExtra(BluetoothDevice.ExtraDevice)!;
                        Debug.WriteLine($"**** BluetoothDevice.ActionFound {device.Name} {device.Address}");
                        break;

                    case BluetoothAdapter.ActionDiscoveryStarted:
                        Debug.WriteLine("**** BluetoothAdapter.ActionDiscoveryStarted");
                        break;
                    case BluetoothAdapter.ActionDiscoveryFinished:
                        Debug.WriteLine("**** BluetoothAdapter.ActionDiscoveryFinished");
                        tcs.SetResult(false);
                        break;

                    case BluetoothDevice.ActionPairingRequest:
                        Debug.WriteLine("**** BluetoothDevice.ActionPairingRequest");
                        break;
                    case BluetoothDevice.ActionBondStateChanged:
                        Debug.WriteLine("**** BluetoothDevice.ActionBondStateChanged");
                        break;
                }
            }
        }
    }
}
