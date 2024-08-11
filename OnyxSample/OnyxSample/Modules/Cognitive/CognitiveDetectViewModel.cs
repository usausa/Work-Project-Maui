namespace OnyxSample.Modules.Cognitive;

public class CognitiveDetectViewModel : AppViewModelBase
{
    private readonly IDispatcher dispatcher;

    public CameraController Camera { get; } = new();

    public NotificationValue<bool> IsPreview { get; } = new(true);

    public NotificationValue<ImageSource?> Image { get; } = new();

    public ICommand DetectCommand { get; }
    //public ICommand TorchCommand { get; }
    //public ICommand MirrorCommand { get; }
    //public ICommand FlashModeCommand { get; }
    //public ICommand ZoomCommand { get; }

    public CognitiveDetectViewModel(
        ApplicationState applicationState,
        IDispatcher dispatcher)
        : base(applicationState)
    {
        this.dispatcher = dispatcher;

        DetectCommand = MakeAsyncCommand(DetectAsync);

        //TorchCommand = MakeDelegateCommand(() => Camera.Torch = !Camera.Torch);
        //MirrorCommand = MakeDelegateCommand(() => Camera.Mirror = !Camera.Mirror);
        //FlashModeCommand = MakeDelegateCommand(SwitchFlashMode);
        //ZoomCommand = MakeDelegateCommand(SwitchZoom, () => Camera.Camera is not null).Observe(Camera);
    }

    protected override Task OnNotifyBackAsync() => Navigator.ForwardAsync(ViewId.Menu);

    protected override Task OnNotifyFunction1() => OnNotifyBackAsync();

    protected async Task DetectAsync()
    {
        if (IsPreview.Value)
        {
            var stream = await Camera.CaptureAsync();
            if (stream is null)
            {
                return;
            }

            await dispatcher.DispatchAsync(async () =>
            {
                Image.Value = ImageSource.FromStream(() => stream);

                await Camera.StopPreviewAsync();
                IsPreview.Value = false;
            });
        }
        else
        {
            await Camera.StartPreviewAsync();
            IsPreview.Value = true;
        }
    }

    protected override Task OnNotifyFunction3()
    {
        Camera.Zoom -= 1f;
        return Task.CompletedTask;
    }

    protected override Task OnNotifyFunction4()
    {
        Camera.Zoom += 1f;
        return Task.CompletedTask;
    }
}
