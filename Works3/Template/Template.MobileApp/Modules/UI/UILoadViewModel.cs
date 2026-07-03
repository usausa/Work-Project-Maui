namespace Template.MobileApp.Modules.UI;

using Template.MobileApp.Components;
using Template.MobileApp.Graphics;

public sealed partial class UILoadViewModel : AppViewModelBase
{
    private static readonly TimeSpan PeakWindow = TimeSpan.FromSeconds(3);

    private readonly INoiseMonitor noiseMonitor;

    private readonly Queue<(DateTime Timestamp, double Value)> peakHistory = new();

    [ObservableProperty]
    public partial double Current { get; set; }

    [ObservableProperty]
    public partial double Average { get; set; }

    [ObservableProperty]
    public partial double Min { get; set; }

    [ObservableProperty]
    public partial double Max { get; set; }

    [ObservableProperty]
    public partial double Peak { get; set; }

    public LoadGraphics Graphics { get; } = new();

    public UILoadViewModel(INoiseMonitor noiseMonitor)
    {
        this.noiseMonitor = noiseMonitor;

        Disposables.Add(noiseMonitor.MeasuredAsObservable().ObserveOnCurrentContext().Subscribe(x =>
        {
            Current = x.Decibel;
            Graphics.AddValue((float)x.Decibel);
            var (avg, min, max) = Graphics.CalcStatics();
            Average = avg;
            Min = min;
            Max = max;
            Peak = CalcPeak(x.Decibel);
        }));
    }

    // 直近 3 秒間の最大値を保持する
    private double CalcPeak(double value)
    {
        var now = DateTime.Now;
        peakHistory.Enqueue((now, value));

        var limit = now - PeakWindow;
        while ((peakHistory.Count > 0) && (peakHistory.Peek().Timestamp < limit))
        {
            peakHistory.Dequeue();
        }

        var peak = 0d;
        foreach (var (_, entryValue) in peakHistory)
        {
            if (entryValue > peak)
            {
                peak = entryValue;
            }
        }
        return peak;
    }

    public override Task OnNavigatedToAsync(INavigationContext context)
    {
        noiseMonitor.Start();
        return Task.CompletedTask;
    }

    public override Task OnNavigatingFromAsync(INavigationContext context)
    {
        noiseMonitor.Stop();
        return Task.CompletedTask;
    }

    protected override Task OnNotifyBackAsync() => Navigator.ForwardAsync(ViewId.UIMenu);

    protected override Task OnNotifyFunction1() => OnNotifyBackAsync();

    protected override Task OnNotifyFunction4()
    {
        Graphics.Clear();
        peakHistory.Clear();
        Current = 0;
        Average = 0;
        Min = 0;
        Max = 0;
        Peak = 0;
        return Task.CompletedTask;
    }
}
