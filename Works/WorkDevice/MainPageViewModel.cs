using Smart.ComponentModel;

namespace WorkDevice;

using Smart.Maui.ViewModels;

public class MainPageViewModel : ViewModelBase
{
    public NotificationValue<string> Data { get; } = new();

    public MainPageViewModel()
    {
        Data.Value = "123";

        UpdateBatteryStatus();
    }

    private void UpdateBatteryStatus()
    {
    }
}
