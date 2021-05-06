namespace WorkKey.FormsApp.Droid.Components.Device
{
    using Android.App;

    using WorkKey.FormsApp.Components.Device;

    public sealed class DeviceManager : DeviceManagerBase
    {
        private readonly Activity activity;

        public DeviceManager(Activity activity)
        {
            this.activity = activity;
        }

        public override string GetVersion()
        {
            var pm = activity.PackageManager;
            var info = pm.GetPackageInfo(activity.PackageName, 0);
            return info.VersionName;
        }
    }
}
