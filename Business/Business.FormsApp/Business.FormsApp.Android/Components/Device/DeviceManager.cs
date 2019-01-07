namespace Business.FormsApp.Droid.Components.Device
{
    using Android.Content;

    using Business.FormsApp.Components.Device;

    public sealed class DeviceManager : DeviceManagerBase
    {
        private readonly Context context;

        private WakeLockBroadcastReceiver receiver;

        public DeviceManager(Context context)
        {
            this.context = context;
        }

        public void RegisterWakeLockReceiver()
        {
            if (receiver == null)
            {
                receiver = new WakeLockBroadcastReceiver(this);
                context.RegisterReceiver(receiver, new IntentFilter(Intent.ActionScreenOn));
                context.RegisterReceiver(receiver, new IntentFilter(Intent.ActionScreenOff));
            }
        }

        public void UnregisterWakeLockReceiver()
        {
            if (receiver != null)
            {
                context.UnregisterReceiver(receiver);
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

        private class WakeLockBroadcastReceiver : BroadcastReceiver
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
                    deviceManager.NotifyScreenState(true);
                }
                else if (intent.Action == Intent.ActionScreenOff)
                {
                    deviceManager.NotifyScreenState(false);
                }
            }
        }
    }
}