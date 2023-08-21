using System.Diagnostics;

namespace WorkCamera;

using System.Windows.Input;

using Camera.MAUI;

using Smart.Maui.ViewModels;

public class MainPageViewModel : ViewModelBase
{
    public CameraController Controller { get; }

    public ICommand StartPreviewCommand { get; }
    public ICommand StopPreviewCommand { get; }

    public ICommand FrontCameraCommand { get; }
    public ICommand BackCameraCommand { get; }
    public ICommand SwitchCameraCommand { get; }

    public ICommand TorchCommand { get; }
    public ICommand MirrorCommand { get; }
    public ICommand FlashModeCommand { get; }
    public ICommand ZoomCommand { get; }

    public MainPageViewModel()
    {
        Controller = new CameraController(command: MakeDelegateCommand<ZXing.Result>(x =>
        {
            Debug.WriteLine($"* {x.BarcodeFormat} {x.Text}");
        }));

        Controller.Preview = true;
        Controller.BarcodeDetection = true;

        StartPreviewCommand = MakeDelegateCommand(() => Controller.Preview = true);
        StopPreviewCommand = MakeDelegateCommand(() => Controller.Preview = false);

        FrontCameraCommand = MakeAsyncCommand(() => Controller.SwitchPositionAsync(CameraPosition.Front));
        BackCameraCommand = MakeAsyncCommand(() => Controller.SwitchPositionAsync(CameraPosition.Back));
        SwitchCameraCommand = MakeAsyncCommand(() => Controller.SwitchPositionAsync());

        TorchCommand = MakeDelegateCommand(() => Controller.Torch = !Controller.Torch);
        MirrorCommand = MakeDelegateCommand(() => Controller.Mirror = !Controller.Mirror);
        FlashModeCommand = MakeDelegateCommand(() => Controller.FlashMode = Controller.FlashMode != FlashMode.Enabled ? FlashMode.Enabled : FlashMode.Disabled);
        ZoomCommand = MakeDelegateCommand(() =>
        {
            if (Controller.Camera is CameraInfo camera)
            {
                Controller.Zoom = Controller.Zoom < camera.MaxZoomFactor ? Controller.Zoom + 1 : 1;
            }
        }, () => Controller.Camera is not null).Observe(Controller);
    }
}