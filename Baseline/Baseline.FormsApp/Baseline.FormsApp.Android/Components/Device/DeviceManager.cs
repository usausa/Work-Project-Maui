namespace Baseline.FormsApp.Droid.Components.Device
{
    using Android.App;
    using Android.Content;
    using Android.Widget;

    using Baseline.FormsApp.Components.Device;

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Ignore")]
    public sealed class DeviceManager : DeviceManagerBase
    {
        private readonly Activity activity;

        private WakeLockBroadcastReceiver receiver;

        public DeviceManager(Activity activity)
        {
            this.activity = activity;
        }

        public void RegisterWakeLockReceiver()
        {
            if (receiver is null)
            {
                receiver = new WakeLockBroadcastReceiver(this);
                activity.RegisterReceiver(receiver, new IntentFilter(Intent.ActionScreenOn));
                activity.RegisterReceiver(receiver, new IntentFilter(Intent.ActionScreenOff));
            }
        }

        public void UnregisterWakeLockReceiver()
        {
            if (receiver != null)
            {
                activity.UnregisterReceiver(receiver);
                receiver = null;
            }
        }

        public void UpdateKeyboardState(bool visible)
        {
            RaiseKeyboardState(visible);
        }

        public void NotifyScreenState(bool on)
        {
            RaiseScreenState(on);
        }

        private sealed class WakeLockBroadcastReceiver : BroadcastReceiver
        {
            private readonly DeviceManager deviceManager;

            public WakeLockBroadcastReceiver(DeviceManager deviceManager)
            {
                this.deviceManager = deviceManager;
            }

            public override void OnReceive(Context context, Intent intent)
            {
                if (intent.Action == Intent.ActionScreenOn)
                {
                    Toast.MakeText(deviceManager.activity, "ON", ToastLength.Short).Show();
                    deviceManager.NotifyScreenState(true);
                }
                else if (intent.Action == Intent.ActionScreenOff)
                {
                    //Toast.MakeText(deviceManager.activity, "OFF", ToastLength.Short).Show();
                    deviceManager.NotifyScreenState(false);
                }
            }
        }

        public override void MoveTaskToBack()
        {
            activity.MoveTaskToBack(true);
        }
    }
}