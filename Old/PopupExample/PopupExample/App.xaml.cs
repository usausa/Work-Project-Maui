namespace PopupExample;

using System.Diagnostics;

public partial class App
{
    public App()
    {
        InitializeComponent();

        MainPage = new MainPage();
        Debug.WriteLine($"Width: {DeviceDisplay.MainDisplayInfo.Width}px");
        Debug.WriteLine($"Height: {DeviceDisplay.MainDisplayInfo.Height}px");
        // 160dpi * density
        Debug.WriteLine($"Density: {DeviceDisplay.MainDisplayInfo.Density}");
        Debug.WriteLine($"Width: {DeviceDisplay.MainDisplayInfo.Width / DeviceDisplay.MainDisplayInfo.Density}");
        Debug.WriteLine($"Height: {DeviceDisplay.MainDisplayInfo.Height / DeviceDisplay.MainDisplayInfo.Density}");
    }
}