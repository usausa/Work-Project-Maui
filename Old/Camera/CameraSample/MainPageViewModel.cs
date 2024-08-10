namespace CameraSample;

using System.Diagnostics;
using System.Windows.Input;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Core.Primitives;

using Smart.ComponentModel;
using Smart.Linq;
using Smart.Maui.ViewModels;

public class MainPageViewModel : ViewModelBase
{
    public NotificationValue<CameraFlashMode> FlashMode { get; } = new();

    public NotificationValue<CameraInfo?> CameraInfo { get; } = new();

    public NotificationValue<Size> Resolution { get; } = new();

    public NotificationValue<float> Zoom { get; } = new();

    public NotificationValue<bool> IsTorchOn { get; } = new();

    public ICollection<CameraFlashMode> FlashModes { get; } = Enum.GetValues<CameraFlashMode>();

    public ICommand ZoomInCommand { get;  }
    public ICommand ZoomOutCommand { get; }

    public ICommand SwitchCommand { get; }

    public MainPageViewModel(ICameraProvider cameraProvider)
    {
        Disposables.Add(CameraInfo.AsValueObservable().Subscribe(x =>
        {
            if (x is not null)
            {
                Debug.WriteLine("--------");
                Debug.WriteLine($"{x.Name}: {x.Position}, {x.IsFlashSupported}, {x.MinimumZoomFactor}, {x.MaximumZoomFactor}");
                foreach (var size in x.SupportedResolutions)
                {
                    Debug.WriteLine(size);
                }
            }
        }));

        ZoomInCommand = MakeDelegateCommand(() => Zoom.Value = Math.Min(Zoom.Value + 1, CameraInfo.Value?.MaximumZoomFactor ?? 1));
        ZoomOutCommand = MakeDelegateCommand(() => Zoom.Value = Math.Max(Zoom.Value - 1, CameraInfo.Value?.MinimumZoomFactor ?? 1));

        SwitchCommand = MakeAsyncCommand(async () =>
        {
            await cameraProvider.RefreshAvailableCameras(default);
            if (CameraInfo.Value is null)
            {
                CameraInfo.Value = cameraProvider.AvailableCameras.FirstOrDefault();
            }
            else
            {
                var index = cameraProvider.AvailableCameras.FindIndex(x => x.DeviceId == CameraInfo.Value.DeviceId);
                if ((index >= 0) && (index < cameraProvider.AvailableCameras.Count - 1))
                {
                    CameraInfo.Value = cameraProvider.AvailableCameras[index + 1];
                }
                else
                {
                    CameraInfo.Value = cameraProvider.AvailableCameras.FirstOrDefault();
                }
            }
        });
    }
}
