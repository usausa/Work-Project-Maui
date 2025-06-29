namespace Template.MobileApp.Modules.UI;

using SkiaSharp;

using Template.MobileApp.Helpers;

public sealed partial class UITreeMapViewModel : AppViewModelBase
{
    [ObservableProperty]
    public partial bool IsPreview { get; set; } = true;

    [ObservableProperty]
    public partial ImageSource? Image { get; set; }

    public CameraController Controller { get; } = new();

    public UITreeMapViewModel()
    {
        Disposables.Add(Controller.AsObservable(nameof(Controller.Selected)).Subscribe(_ =>
        {
            // Select minimum resolution
            var resolutions = Controller.Selected?.SupportedResolutions ?? [];
            var size = Size.Zero;
            foreach (var resolution in resolutions)
            {
                if ((resolution.Width < size.Width) || (resolution.Height < size.Height) || size.IsZero)
                {
                    size = resolution;
                }
            }

            Controller.CaptureResolution = size;
        }));
    }

    public override async Task OnNavigatedToAsync(INavigationContext context)
    {
        if (IsPreview)
        {
            await Controller.StartPreviewAsync().ConfigureAwait(true);
        }
    }

    public override async Task OnNavigatingFromAsync(INavigationContext context)
    {
        if (IsPreview)
        {
            await Controller.StopPreviewAsync().ConfigureAwait(true);
        }
    }

    protected override Task OnNotifyBackAsync() => Navigator.ForwardAsync(ViewId.UIMenu);

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

    protected override Task OnNotifyFunction4()
    {
        return BusyState.Using(async () =>
        {
            if (IsPreview)
            {
                // Capture
                await using var input = await Controller.CaptureAsync().ConfigureAwait(true);
                if (input is null)
                {
                    return;
                }

                await Controller.StopPreviewAsync().ConfigureAwait(true);

                // Bitmap
                using var bitmap = ImageHelper.ToNormalizeBitmap(input);

                var data = bitmap.Encode(SKEncodedImageFormat.Jpeg, 100);
                Image = ImageSource.FromStream(() => data.AsStream());

                // TODO

                IsPreview = false;
            }
            else
            {
                await Controller.StartPreviewAsync().ConfigureAwait(true);
                IsPreview = true;
            }
        });
    }
}
