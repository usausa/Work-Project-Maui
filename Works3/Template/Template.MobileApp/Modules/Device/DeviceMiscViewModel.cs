namespace Template.MobileApp.Modules.Device;

using Template.MobileApp.Components.Device;

public class DeviceMiscViewModel : AppViewModelBase
{
    public ICommand VibrateCommand { get; }

    public ICommand VibrateCancelCommand { get; }

    public ICommand LightOnCommand { get; }

    public ICommand LightOffCommand { get; }

    public DeviceMiscViewModel(
        ApplicationState applicationState,
        IDeviceManager device)
        : base(applicationState)
    {
        VibrateCommand = MakeDelegateCommand(() => device.Vibrate(5000));
        VibrateCancelCommand = MakeDelegateCommand(device.VibrateCancel);
        LightOnCommand = MakeDelegateCommand(device.LightOn);
        LightOffCommand = MakeDelegateCommand(device.LightOff);
    }

    protected override Task OnNotifyBackAsync() => Navigator.ForwardAsync(ViewId.DeviceMenu);
}
