namespace Business.FormsApp.Modules.Platform
{
    using System.Threading.Tasks;

    using Smart.ComponentModel;
    using Smart.Navigation;

    using Xamarin.Essentials;

    public class PlatformInfoViewModel : AppViewModelBase
    {
        public NotificationValue<string> AppName { get; } = new NotificationValue<string>();
        public NotificationValue<string> AppPackageName { get; } = new NotificationValue<string>();
        public NotificationValue<string> AppVersionString { get; } = new NotificationValue<string>();

        public NotificationValue<string> DeviceModel { get; } = new NotificationValue<string>();
        public NotificationValue<string> DeviceManufacturer { get; } = new NotificationValue<string>();
        public NotificationValue<string> DeviceName { get; } = new NotificationValue<string>();
        public NotificationValue<string> DeviceVersionString { get; } = new NotificationValue<string>();
        public NotificationValue<string> DevicePlatform { get; } = new NotificationValue<string>();
        public NotificationValue<string> DeviceIdiom { get; } = new NotificationValue<string>();
        public NotificationValue<string> DeviceType { get; } = new NotificationValue<string>();

        public NotificationValue<string> DisplayOrientation { get; } = new NotificationValue<string>();
        public NotificationValue<string> DisplayRotation { get; } = new NotificationValue<string>();
        public NotificationValue<string> DisplaySize { get; } = new NotificationValue<string>();
        public NotificationValue<string> DisplayDensity { get; } = new NotificationValue<string>();

        public PlatformInfoViewModel(ApplicationState applicationState)
            : base(applicationState)
        {
            AppName.Value = AppInfo.Name;
            AppPackageName.Value = AppInfo.PackageName;
            AppVersionString.Value = AppInfo.VersionString;

            DeviceModel.Value = DeviceInfo.Model;
            DeviceManufacturer.Value = DeviceInfo.Manufacturer;
            DeviceName.Value = DeviceInfo.Name;
            DeviceVersionString.Value = DeviceInfo.VersionString;
            DevicePlatform.Value = DeviceInfo.Platform.ToString();
            DeviceIdiom.Value = DeviceInfo.Idiom.ToString();
            DeviceType.Value = DeviceInfo.DeviceType.ToString();

            var mdi = DeviceDisplay.MainDisplayInfo;
            DisplayOrientation.Value = mdi.Orientation.ToString();
            DisplayRotation.Value = mdi.Rotation.ToString();
            DisplaySize.Value = $"{mdi.Width}, {mdi.Height}";
            DisplayDensity.Value = $"{mdi.Density}";
        }

        protected override Task OnNotifyBackAsync()
        {
            return Navigator.ForwardAsync(ViewId.PlatformMenu);
        }
    }
}
