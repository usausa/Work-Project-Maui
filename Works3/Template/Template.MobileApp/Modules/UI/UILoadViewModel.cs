namespace Template.MobileApp.Modules.UI;

using Template.MobileApp.Components;
using Template.MobileApp.Graphics;

public sealed partial class UILoadViewModel : AppViewModelBase
{
    private readonly INoiseMonitor noiseMonitor;

    [ObservableProperty]
    public partial double Current { get; set; }

    [ObservableProperty]
    public partial double Average { get; set; }

    [ObservableProperty]
    public partial double Min { get; set; }

    [ObservableProperty]
    public partial double Max { get; set; }

    public LoadGraphics Graphics { get; } = new();

    public UILoadViewModel(INoiseMonitor noiseMonitor)
    {
        this.noiseMonitor = noiseMonitor;

        Disposables.Add(noiseMonitor.ObserveMeasuredOnCurrentContext().Subscribe(x =>
        {
            Current = x.Decibel;
            Graphics.AddValue((float)x.Decibel);
            var (avg, min, max) = Graphics.CalcStatics();
            Average = avg;
            Min = min;
            Max = max;
        }));
    }

    public override void OnNavigatedTo(INavigationContext context)
    {
        noiseMonitor.Start();
    }

    public override void OnNavigatingFrom(INavigationContext context)
    {
        noiseMonitor.Stop();
    }

    protected override Task OnNotifyBackAsync() => Navigator.ForwardAsync(ViewId.UIMenu);

    protected override Task OnNotifyFunction1() => OnNotifyBackAsync();

    protected override Task OnNotifyFunction4()
    {
        Graphics.Clear();
        Current = 0;
        Average = 0;
        Min = 0;
        Max = 0;
        return Task.CompletedTask;
    }
}
