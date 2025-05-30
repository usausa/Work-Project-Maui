using System.Diagnostics;
using System.Windows.Input;
using CommunityToolkit.Maui.Core.Primitives;
using Smart;
using Smart.Linq;

namespace WorkNewCamera;

using Smart.Maui.ViewModels;

public class MainPageViewModel : ExtendViewModelBase
{
    public CameraController Controller { get; } = new();

    public ICommand InfoCommand { get; }
    public ICommand ListCommand { get; }
    public ICommand FlipCommand { get; }
    public ICommand TorchCommand { get; }
    public ICommand FlashCommand { get; }
    //public ICommand SizeCommand { get; }
    //public ICommand ZoomCommand { get; }
    //public ICommand StartCommand { get; }
    //public ICommand StopCommand { get; }
    //public ICommand CaptureCommand { get; }

    public MainPageViewModel()
    {
        InfoCommand = MakeDelegateCommand(() =>
        {
            var camera = Controller.Selected;
            if (camera is null)
            {
                Debug.WriteLine("* none");
                return;
            }
            Debug.WriteLine($"* Id: {camera.DeviceId}");
            Debug.WriteLine($"* Name: {camera.Name}");
            Debug.WriteLine($"* Position: {camera.Position}");
            Debug.WriteLine($"* Min: {camera.MinimumZoomFactor}");
            Debug.WriteLine($"* Max: {camera.MaximumZoomFactor}");
            Debug.WriteLine($"* Flash: {camera.IsFlashSupported}");
            foreach (var size in camera.SupportedResolutions)
            {
                Debug.WriteLine($"* Size: {size}");
            }
        });
        ListCommand = MakeAsyncCommand(async () =>
        {
            var list = await Controller.GetAvailableListAsync();
            Debug.WriteLine($"* Count: {list.Count}");
        });
        FlipCommand = MakeAsyncCommand(async () =>
        {
            var list = await Controller.GetAvailableListAsync();
            if (Controller.Selected is null)
            {
                Controller.Selected = list.FirstOrDefault();
            }
            else
            {
                var index = list.FindIndex(x => x.DeviceId == Controller.Selected.DeviceId);
                if ((index < 0) || (index == list.Count - 1))
                {
                    Controller.Selected = list.FirstOrDefault();
                }
                else
                {
                    Controller.Selected = list[index + 1];
                }

            }
        });
        TorchCommand = MakeDelegateCommand(() => Controller.IsTorchOn = !Controller.IsTorchOn);
        FlashCommand = MakeDelegateCommand(() =>
        {
            if (Controller.Selected?.IsFlashSupported ?? false)
            {
                switch (Controller.CameraFlashMode)
                {
                    case CameraFlashMode.Off:
                        Controller.CameraFlashMode = CameraFlashMode.On;
                        break;
                    case CameraFlashMode.On:
                        Controller.CameraFlashMode = CameraFlashMode.Auto;
                        break;
                    case CameraFlashMode.Auto:
                        Controller.CameraFlashMode = CameraFlashMode.Off;
                        break;
                }
            }
        });
    }
}

// TODO Ext switch camera zoom up down