namespace Template.MobileApp.Modules.Network;

using BarcodeScanning;

using Template.MobileApp.Services;

public sealed partial class NetworkSettingViewModel : AppViewModelBase
{
    public BarcodeController Controller { get; } = new();

    [ObservableProperty]
    public partial string Current { get; set; }

    public IObserveCommand DetectCommand { get; }

    public NetworkSettingViewModel(
        ApiContext apiContext,
        Settings settings,
        IDispatcher dispatcher,
        IDialog dialog)
    {
        Controller.AimMode = true;
        Controller.VibrationOnDetect = true;
        Controller.CaptureNextFrame = false;

        Current = settings.ApiEndPoint;

        DetectCommand = MakeDelegateCommand<IReadOnlySet<BarcodeResult>>(x =>
        {
            if (!Controller.Enable)
            {
                return;
            }

            if (x.Count > 0)
            {
                Controller.Enable = false;

                // ReSharper disable once AsyncVoidLambda
                dispatcher.Dispatch(async () =>
                {
                    var barcode = x.First().DisplayValue;
                    try
                    {
                        var url = new Uri(barcode);
                        if (await dialog.ConfirmAsync($"Update ?\n{barcode}"))
                        {
                            settings.ApiEndPoint = barcode;
                            apiContext.BaseAddress = url;
                            //await Navigator.ForwardAsync(ViewId.NetworkMenu);
                            return;
                        }
                    }
                    catch (UriFormatException)
                    {
                        await dialog.InformationAsync("Invalid url.");
                    }

                    Controller.Enable = true;
                });
            }
        });

        //DetectCommand = MakeAsyncCommand<IReadOnlySet<BarcodeResult>>(async x =>
        //{
        //    if (!Controller.Enable)
        //    {
        //        return;
        //    }

        //    if (x.Count > 0)
        //    {
        //        Controller.Enable = false;

        //        var barcode = x.First().DisplayValue;
        //        try
        //        {
        //            var url = new Uri(barcode);
        //            if (await dialog.ConfirmAsync($"Update ?\n{barcode}"))
        //            {
        //                settings.ApiEndPoint = barcode;
        //                apiContext.BaseAddress = url;
        //                return;
        //            }
        //        }
        //        catch (UriFormatException)
        //        {
        //            await dialog.InformationAsync("Invalid url.");
        //        }

        //        Controller.Enable = true;
        //    }
        //});
    }

    public override void OnNavigatedTo(INavigationContext context)
    {
        Controller.Enable = true;
    }

    public override void OnNavigatingFrom(INavigationContext context)
    {
        Controller.Enable = false;
    }

    protected override Task OnNotifyBackAsync() => Navigator.ForwardAsync(ViewId.NetworkMenu);

    protected override Task OnNotifyFunction1() => OnNotifyBackAsync();

    protected override Task OnNotifyFunction4()
    {
        Controller.Enable = true;
        return Task.CompletedTask;
    }
}
