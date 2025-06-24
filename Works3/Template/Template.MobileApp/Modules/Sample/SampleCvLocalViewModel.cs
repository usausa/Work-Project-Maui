namespace Template.MobileApp.Modules.Sample;

using Template.MobileApp.Graphics;
using Template.MobileApp.Helpers;
using Template.MobileApp.Usecase;

public sealed partial class SampleCvLocalViewModel : AppViewModelBase
{
    private readonly CognitiveUsecase cognitiveUsecase;

    [ObservableProperty]
    public partial bool IsPreview { get; set; } = true;

    [ObservableProperty]
    public partial ImageSource? Image { get; set; }

    public CameraController Controller { get; } = new();

    public DetectGraphics Graphics { get; } = new();

    public SampleCvLocalViewModel(
        CognitiveUsecase cognitiveUsecase)
    {
        this.cognitiveUsecase = cognitiveUsecase;
    }

    public override async Task OnNavigatedToAsync(INavigationContext context)
    {
        if (IsPreview)
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

    protected override Task OnNotifyBackAsync() => Navigator.ForwardAsync(ViewId.SampleMenu);

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
            await using var input = await Controller.CaptureAsync();
            if (input is null)
            {
                return;
            }

            await Controller.StopPreviewAsync();

            var ms = ImageHelper.ToMemoryStream(input);

            // TODO
            var copy = new MemoryStream();
            await ms.CopyToAsync(copy);
            ms.Seek(0, SeekOrigin.Begin);
            copy.Seek(0, SeekOrigin.Begin);

            var results = await cognitiveUsecase.DetectAsync(ms);
            Graphics.Update(results);

            Image = ImageSource.FromStream(() => copy);

            IsPreview = false;
        }
        else
        {
            await Controller.StartPreviewAsync();
            IsPreview = true;
        }
    }
}
