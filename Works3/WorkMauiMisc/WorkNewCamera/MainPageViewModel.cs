using System.Diagnostics;
using System.Windows.Input;

namespace WorkNewCamera;

using Smart.Maui.ViewModels;

public class MainPageViewModel : ExtendViewModelBase
{
    public CameraController Controller { get; } = new();

    public ICommand InfoCommand { get; }
    //public ICommand FlipCommand { get; }
    //public ICommand FlashCommand { get; }
    //public ICommand SizeCommand { get; }
    //public ICommand ZoomCommand { get; }
    //public ICommand TorchCommand { get; }
    public ICommand ListCommand { get; }
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
    }
}
