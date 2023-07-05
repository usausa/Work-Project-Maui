namespace Template.MobileApp.Modules.Device;

using Template.MobileApp.Components.Device;
using Template.MobileApp.Components.Sound;

public class DeviceMiscViewModel : AppViewModelBase
{
    public ICommand VibrateCommand { get; }
    public ICommand VibrateCancelCommand { get; }

    public ICommand LightOnCommand { get; }
    public ICommand LightOffCommand { get; }

    public ICommand SpeakCommand { get; }
    public ICommand SpeakCancelCommand { get; }

    public DeviceMiscViewModel(
        ApplicationState applicationState,
        IDeviceManager device,
        ISoundManager soundManager)
        : base(applicationState)
    {
        VibrateCommand = MakeDelegateCommand(() => device.Vibrate(5000));
        VibrateCancelCommand = MakeDelegateCommand(device.VibrateCancel);
        LightOnCommand = MakeDelegateCommand(device.LightOn);
        LightOffCommand = MakeDelegateCommand(device.LightOff);
#pragma warning disable CA2012
        SpeakCommand = MakeDelegateCommand(() => soundManager.SpeakAsync("テストです"));
#pragma warning restore CA2012
        SpeakCancelCommand = MakeDelegateCommand(soundManager.SpeakCancel);
    }

    protected override Task OnNotifyBackAsync() => Navigator.ForwardAsync(ViewId.DeviceMenu);
}
