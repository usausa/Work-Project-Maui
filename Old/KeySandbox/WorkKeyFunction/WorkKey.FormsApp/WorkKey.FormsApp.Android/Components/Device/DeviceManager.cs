namespace WorkKey.FormsApp.Droid.Components.Device
{
    using Android.App;
    using Android.Views;

    using WorkKey.FormsApp.Components.Device;
    using WorkKey.FormsApp.Shell;

    public sealed class DeviceManager : DeviceManagerBase
    {
        private readonly Activity activity;

        public DeviceManager(Activity activity)
        {
            this.activity = activity;
        }

        public bool DispatchKeyEvent(KeyEvent e)
        {
            if ((e.KeyCode >= Keycode.F1) && (e.KeyCode <= Keycode.F4))
            {
                if (e.Action == KeyEventActions.Up)
                {
                    RaiseShellKeyDown(ShellEvent.Function1 + (e.KeyCode - Keycode.F1));
                }

                return true;
            }

            return false;
        }

        public override string GetVersion()
        {
            var pm = activity.PackageManager;
            var info = pm.GetPackageInfo(activity.PackageName, 0);
            return info.VersionName;
        }
    }
}
