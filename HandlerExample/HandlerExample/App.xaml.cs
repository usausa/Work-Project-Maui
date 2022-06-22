namespace HandlerExample;

using Microsoft.Maui.Controls.Compatibility.Platform.Android;
using Microsoft.Maui.Handlers;

public partial class App
{
    public App()
    {
        InitializeComponent();

#if ANDROID
        EntryHandler.Mapper.AppendToMapping(nameof(IView.Background), (handler, view) =>
        {
            if (view is CustomEntry2)
            {
                handler.PlatformView.SetTextColor(Android.Graphics.Color.White);
                handler.PlatformView.SetBackgroundColor(Android.Graphics.Color.Blue);
            }
        });
#endif

        MainPage = new MainPage();
    }
}
