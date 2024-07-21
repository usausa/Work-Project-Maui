namespace WorkBarcode;

using System.Diagnostics;

using ZXing.Net.Maui;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();

        CameraBarcodeReaderView.Options = new BarcodeReaderOptions
        {
            AutoRotate = true,
            Multiple = true
        };
    }

    private void CameraBarcodeReaderView_OnBarcodesDetected(object? sender, BarcodeDetectionEventArgs e)
    {
        Dispatcher.Dispatch(() =>
        {
            foreach (var barcode in e.Results)
            {
                Debug.WriteLine($"{barcode.Format} {barcode.Value}");
                ResultLabel.Text = $"{barcode.Format} {barcode.Value}";
                foreach (var point in barcode.PointsOfInterest)
                {
                    Debug.WriteLine($"  {point.X} {point.Y}");
                }

                Vibration.Default.Vibrate(TimeSpan.FromMilliseconds(100));
            }
        });
    }
}

