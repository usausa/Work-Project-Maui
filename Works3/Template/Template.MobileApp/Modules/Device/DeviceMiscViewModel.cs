#pragma warning disable SA1135
namespace Template.MobileApp.Modules.Device;

using Template.MobileApp.Components.Device;
using Template.MobileApp.Components.Sound;
using Template.MobileApp.Components.Storage;

public class DeviceMiscViewModel : AppViewModelBase
{
    public ICommand VibrateCommand { get; }
    public ICommand VibrateCancelCommand { get; }

    public ICommand LightOnCommand { get; }
    public ICommand LightOffCommand { get; }

    public ICommand SpeakCommand { get; }
    public ICommand SpeakCancelCommand { get; }

    public ICommand ScreenshotCommand { get; }

    public DeviceMiscViewModel(
        ApplicationState applicationState,
        IDeviceManager device,
        IStorageManager storage,
        ISoundManager sound)
        : base(applicationState)
    {
        VibrateCommand = MakeDelegateCommand(() => device.Vibrate(5000));
        VibrateCancelCommand = MakeDelegateCommand(device.VibrateCancel);
        LightOnCommand = MakeDelegateCommand(device.LightOn);
        LightOffCommand = MakeDelegateCommand(device.LightOff);
#pragma warning disable CA2012
        SpeakCommand = MakeDelegateCommand(() => sound.SpeakAsync("テストです"));
#pragma warning restore CA2012
        SpeakCancelCommand = MakeDelegateCommand(sound.SpeakCancel);
        ScreenshotCommand = MakeAsyncCommand(async () =>
        {
            await using var stream = await device.TakeScreenshotAsync();
            await using var file = File.Create(Path.Combine(storage.PublicFolder, "screenshot.jpg"));
            await stream.CopyToAsync(file);
        });
    }

    protected override Task OnNotifyBackAsync() => Navigator.ForwardAsync(ViewId.DeviceMenu);
}
