namespace Business.FormsApp.Modules.Platform
{
    using System;
    using System.Threading.Tasks;

    using Business.FormsApp.Components.Device;

    using Smart.ComponentModel;
    using Smart.Navigation;

    public class PlatformStatusViewModel : AppViewModelBase
    {
        public static PlatformStatusViewModel DesignInstance { get; } = null; // For design

        public NotificationValue<string> BatteryLevel { get; } = new NotificationValue<string>();

        public NotificationValue<string> WifiConnected { get; } = new NotificationValue<string>();

        public PlatformStatusViewModel(
            ApplicationState applicationState,
            IDeviceManager deviceManager)
            : base(applicationState)
        {
            Disposables.Add(deviceManager.BatteryLevel
                .Subscribe(x => BatteryLevel.Value = $"{x * 100}"));
            Disposables.Add(deviceManager.WifiConnected
                .Subscribe(x => WifiConnected.Value = $"{x}"));
        }

        protected override Task OnNotifyBackAsync()
        {
            return Navigator.ForwardAsync(ViewId.PlatformMenu);
        }
    }
}
