namespace WorkSocial;

using System.Diagnostics;

public partial class App : Application
{
    public App()
    {
        Current!.UserAppTheme = AppTheme.Light;

        InitializeComponent();

        Debug.WriteLine($"* Density : {DeviceDisplay.MainDisplayInfo.Density}");
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        return new Window(new MainPage());
    }
}