namespace WorkQrDisplay1;

using QRCoder;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();

        using var generator = new QRCodeGenerator();
        using var data = generator.CreateQrCode("うさうさだよもん", QRCodeGenerator.ECCLevel.L, true);
        var png = new PngByteQRCode(data);
        var bytes = png.GetGraphic(20);
        Image.Source = ImageSource.FromStream(() => new MemoryStream(bytes));
    }
}

