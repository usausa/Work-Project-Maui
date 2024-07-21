namespace WorkBarcode;
using Microsoft.Extensions.Logging;

using ZXing.Net.Maui.Controls;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseBarcodeReader();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
