namespace Template.MobileApp.Modules.Device;

using Template.MobileApp.Components.Device;

public class DeviceMiscViewModel : AppViewModelBase
{
    public ICommand VibrateCommand { get; }

    public ICommand VibrateCancelCommand { get; }

    public ICommand BackCommand { get; }

    public DeviceMiscViewModel(
        ApplicationState applicationState,
        IDeviceManager device)
        : base(applicationState)
    {
        VibrateCommand = MakeDelegateCommand(() => device.Vibrate(5000));
        VibrateCancelCommand = MakeDelegateCommand(device.VibrateCancel);

        BackCommand = MakeAsyncCommand(async () => await Navigator.ForwardAsync(ViewId.DeviceMenu));
    }
}
