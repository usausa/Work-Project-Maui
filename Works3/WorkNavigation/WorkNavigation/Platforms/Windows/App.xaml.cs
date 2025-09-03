namespace WorkNavigation.WinUI;

using Microsoft.UI.Xaml;

using Windows.Graphics;

public partial class App : MauiWinUIApplication
{
    public App()
    {
        this.InitializeComponent();

        var windowWidth = 480;
        var windowHeight = 800;
        Microsoft.Maui.Handlers.WindowHandler.Mapper.AppendToMapping(nameof(IWindow), (handler, view) =>
        {
            var nativeWindow = handler.PlatformView;
            nativeWindow.Activate();
            var windowHandle = WinRT.Interop.WindowNative.GetWindowHandle(nativeWindow);
            var windowId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(windowHandle);
            var appWindow = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(windowId);
            appWindow.Resize(new SizeInt32(windowWidth, windowHeight));
        });
    }

    protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
}
