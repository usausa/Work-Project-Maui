namespace CameraSample;

using System.Diagnostics;
using CommunityToolkit.Maui.Views;

public partial class MainPage
{
    public MainPage()
    {
        InitializeComponent();
    }

    private async void Button_OnClicked(object? sender, EventArgs e)
    {
        CameraImage.Source = null;
        await CameraView.CaptureImage(CancellationToken.None);
        Debug.WriteLine("* CaptureImage started.");
    }

    private void CameraView_OnMediaCaptured(object? sender, MediaCapturedEventArgs e)
    {
        Debug.WriteLine("* Media captured.");
        Dispatcher.Dispatch(() =>
        {
            CameraImage.Source = ImageSource.FromStream(() => e.Media);
        });
    }

    private void CameraView_OnMediaCaptureFailed(object? sender, MediaCaptureFailedEventArgs e)
    {
        Debug.WriteLine("* Media capture failed.");
    }
}

