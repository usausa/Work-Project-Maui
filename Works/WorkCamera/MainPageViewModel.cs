namespace WorkCamera;

using System.Collections.ObjectModel;

using Camera.MAUI;

using Smart.ComponentModel;
using Smart.Maui.ViewModels;

public class MainPageViewModel : ViewModelBase
{
    public NotificationValue<ObservableCollection<CameraInfo>> Cameras { get; } = new();

    public NotificationValue<CameraInfo?> Camera { get; } = new();

    public NotificationValue<bool> Preview { get; } = new();

    public NotificationValue<int> NumCamerasDetected { get; } = new();

    public MainPageViewModel()
    {
        //NumCamerasDetected.AsObservable().Subscribe(x =>
        //{
        //    if (Camera.Value is null)
        //    {
        //        Preview.Value = false;
        //        Camera.Value = Cameras.Value.FirstOrDefault();
        //        Preview.Value = true;
        //    }
        //});
    }
}
