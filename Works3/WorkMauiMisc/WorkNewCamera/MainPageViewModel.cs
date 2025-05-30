namespace WorkNewCamera;

using System.Diagnostics;
using System.Windows.Input;

using CommunityToolkit.Maui.Core.Primitives;

using Smart.Linq;
using Smart.Maui.ViewModels;

public class MainPageViewModel : ExtendViewModelBase
{
    public CameraController Controller { get; } = new();

    public ICommand InfoCommand { get; }
    public ICommand ListCommand { get; }
    public ICommand FlipCommand { get; }
    public ICommand TorchCommand { get; }
    public ICommand FlashCommand { get; }
    public ICommand ZoomInCommand { get; }
    public ICommand ZoomOutCommand { get; }
    public ICommand StartCommand { get; }
    public ICommand StopCommand { get; }
    public ICommand CaptureCommand { get; }

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
        FlipCommand = MakeAsyncCommand(async () => await Controller.SwitchCameraAsync());
        TorchCommand = MakeDelegateCommand(() => Controller.IsTorchOn = !Controller.IsTorchOn);
        FlashCommand = MakeDelegateCommand(() => { Controller.SwitchFlashMode(); });
        ZoomInCommand = MakeDelegateCommand(() => Controller.ZoomIn());
        ZoomOutCommand = MakeDelegateCommand(() => Controller.ZoomOut());
        StartCommand = MakeAsyncCommand(async () => await Controller.StartPreviewAsync());
        StopCommand = MakeAsyncCommand(async () => await Controller.StopPreviewAsync());
        CaptureCommand = MakeAsyncCommand(async () =>
        {
            await using var stream = await Controller.CaptureAsync();
            if (stream is not null)
            {
                Debug.WriteLine(stream.Length);
            }
        });
    }
}

public static class CameraControllerExtensions
{
    public static async ValueTask SwitchCameraAsync(this CameraController controller)
    {
        var list = await controller.GetAvailableListAsync();
        if (controller.Selected is null)
        {
            controller.Selected = list.FirstOrDefault();
        }
        else
        {
            var index = list.FindIndex(x => x.DeviceId == controller.Selected.DeviceId);
            if ((index < 0) || (index == list.Count - 1))
            {
                controller.Selected = list.FirstOrDefault();
            }
            else
            {
                controller.Selected = list[index + 1];
            }
        }
    }

    public static void SwitchFlashMode(this CameraController controller)
    {
        if (controller.Selected?.IsFlashSupported ?? false)
        {
            switch (controller.CameraFlashMode)
            {
                case CameraFlashMode.Off:
                    controller.CameraFlashMode = CameraFlashMode.On;
                    break;
                case CameraFlashMode.On:
                    controller.CameraFlashMode = CameraFlashMode.Auto;
                    break;
                case CameraFlashMode.Auto:
                    controller.CameraFlashMode = CameraFlashMode.Off;
                    break;
            }
        }
    }

    public static void ZoomIn(this CameraController controller)
    {
        var camera = controller.Selected;
        if (camera is null)
        {
            controller.ZoomFactor = 1;
            return;
        }

        controller.ZoomFactor = Math.Min((float)Math.Floor(camera.MaximumZoomFactor), controller.ZoomFactor + 1);
    }

    public static void ZoomOut(this CameraController controller)
    {
        var camera = controller.Selected;
        if (camera is null)
        {
            controller.ZoomFactor = 1;
            return;
        }

        controller.ZoomFactor = Math.Max(1, controller.ZoomFactor - 1);
    }
}

// TODO Ext switch camera zoom up down