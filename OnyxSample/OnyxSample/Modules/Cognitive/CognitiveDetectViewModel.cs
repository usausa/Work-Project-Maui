namespace OnyxSample.Modules.Cognitive;

using System.Diagnostics;

using OnyxSample.Helpers;
using OnyxSample.Usecase;

using SkiaSharp;

public class CognitiveDetectViewModel : AppViewModelBase
{
    private readonly IDispatcher dispatcher;

    private readonly CognitiveUsecase cognitiveUsecase;

    public CameraController Camera { get; } = new();

    public NotificationValue<bool> IsPreview { get; } = new(true);

    public NotificationValue<ImageSource?> Image { get; } = new();

    public ICommand DetectCommand { get; }

    public CognitiveDetectViewModel(
        ApplicationState applicationState,
        IDispatcher dispatcher,
        CognitiveUsecase cognitiveUsecase)
        : base(applicationState)
    {
        this.dispatcher = dispatcher;
        this.cognitiveUsecase = cognitiveUsecase;

        DetectCommand = MakeAsyncCommand(DetectAsync);

        Disposables.Add(Camera.AsObservable(nameof(CameraController.CameraInfo)).Subscribe(x =>
        {
            if (x.CameraInfo is not null)
            {
                Camera.Resolution = x.CameraInfo.SupportedResolutions
                    .Where(static s => s.Width > s.Height)
                    .MinBy(s => s.Height);
            }
        }));
    }

    protected override Task OnNotifyBackAsync() => Navigator.ForwardAsync(ViewId.Menu);

    protected override Task OnNotifyFunction1() => OnNotifyBackAsync();

    protected async Task DetectAsync()
    {
        if (IsPreview.Value)
        {
            using var cts = new CancellationTokenSource();
            cts.CancelAfter(TimeSpan.FromSeconds(5));
            var stream = await Camera.CaptureAsync(cts.Token);
            if (stream is null)
            {
                return;
            }

            using var bitmap = ImageHelper.ToNormalizeBitmap(stream);
            var results = await cognitiveUsecase.DetectAsync(bitmap);

            using var canvas = new SKCanvas(bitmap);
            using var paint = new SKPaint();
            paint.Color = SKColors.Red;
            paint.StrokeWidth = 5;
            paint.IsStroke = true;
            foreach (var result in results)
            {
                Debug.WriteLine($"{result.Score} : {result.Left} {result.Top} {result.Right} {result.Bottom}");
                if (result.Score >= 0.5)
                {
                    canvas.DrawRect(new SKRect(bitmap.Width * result.Left, bitmap.Height * result.Top, bitmap.Width * result.Right, bitmap.Height * result.Bottom), paint);
                }
            }

            var output = ImageHelper.ToImageStream(bitmap);
            await dispatcher.DispatchAsync(async () =>
            {
                Image.Value = ImageSource.FromStream(() => output);

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
