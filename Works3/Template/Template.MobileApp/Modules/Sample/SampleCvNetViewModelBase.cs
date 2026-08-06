namespace Template.MobileApp.Modules.Sample;

using Template.MobileApp.Helpers;

// CvNet系サンプル画面の共通骨格 (プレビュー・ズーム・キャプチャ)。派生側は推論呼び出しと結果反映のみ実装する
public abstract partial class SampleCvNetViewModelBase : AppViewModelBase
{
    [ObservableProperty]
    public partial bool IsPreview { get; set; } = true;

    public SKBitmapImageSource Image { get; } = new();

    public CameraController Controller { get; } = new();

    protected SampleCvNetViewModelBase()
    {
        Disposables.Add(Controller.AsObservable(nameof(Controller.Selected)).Subscribe(_ => Controller.SelectMinimumResolution()));
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            Image.ReplaceBitmap(null);
        }

        base.Dispose(disposing);
    }

    public override async Task OnNavigatedToAsync(INavigationContext context)
    {
        if (IsPreview && await Permissions.RequestCameraAsync())
        {
            await Controller.StartPreviewAsync();
        }
    }

    public override async Task OnNavigatingFromAsync(INavigationContext context)
    {
        if (IsPreview)
        {
            await Controller.StopPreviewAsync();
        }
    }

    protected override Task OnNotifyBackAsync() => Navigator.ForwardAsync(ViewId.SampleCvNetMenu);

    protected override Task OnNotifyFunction1() => OnNotifyBackAsync();

    protected override Task OnNotifyFunction2()
    {
        Controller.ZoomOut();
        return Task.CompletedTask;
    }

    protected override Task OnNotifyFunction3()
    {
        Controller.ZoomIn();
        return Task.CompletedTask;
    }

    protected override async Task OnNotifyFunction4()
    {
        if (IsPreview)
        {
            // Capture
            await using var input = await Controller.CaptureAsync().ConfigureAwait(true);
            if (input is null)
            {
                return;
            }

            await Controller.StopPreviewAsync();

            // Bitmap
            var bitmap = ImageHelper.ToNormalizeBitmap(input);
            Image.ReplaceBitmap(bitmap);

            await OnCapturedAsync(bitmap).ConfigureAwait(true);
        }
        else
        {
            await Controller.StartPreviewAsync();
        }

        IsPreview = !IsPreview;
    }

    // キャプチャ後の推論処理。bitmapの所有権はImage側にあるため派生側で解放しないこと
    protected virtual ValueTask OnCapturedAsync(SKBitmap bitmap) => ValueTask.CompletedTask;
}
