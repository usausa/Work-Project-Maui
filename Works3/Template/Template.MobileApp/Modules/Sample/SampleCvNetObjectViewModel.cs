namespace Template.MobileApp.Modules.Sample;

public sealed partial class SampleCvNetObjectViewModel : SampleCvNetViewModelBase
{
    // TODO
    //private readonly CognitiveUsecase cognitiveUsecase;

    [ObservableProperty]
    public partial int CaptureCount { get; set; }

    // TODO
    //public DetectDrawing Drawing { get; } = new();

    protected override ValueTask OnCapturedAsync(SKBitmap bitmap)
    {
        // シャッターフラッシュのトリガー
        CaptureCount++;

        // TODO
        return ValueTask.CompletedTask;
    }
}
