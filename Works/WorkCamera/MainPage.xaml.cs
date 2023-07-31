namespace WorkCamera;

using Android.Service.Controls.Templates;
using Camera.MAUI;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
    }

    private void CameraView_OnCamerasLoaded(object sender, EventArgs e)
    {
        CameraView.Camera = CameraView.Cameras.FirstOrDefault();
        MainThread.BeginInvokeOnMainThread(async () =>
        {
            if (await CameraView.StartCameraAsync() == CameraResult.Success)
            {
            }
        });
    }
}

