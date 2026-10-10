namespace Template.MobileApp.Modules.UI;

#pragma warning disable CA5394
public sealed partial class UIRadarViewModel : AppViewModelBase
{
    private readonly Random random = new();

    private readonly IDispatcherTimer timer;

    [ObservableProperty(NotifyAlso = [nameof(TargetCount)])]
    public partial IReadOnlyList<RadarTarget> Targets { get; set; }

    public int TargetCount => Targets.Count;

    //--------------------------------------------------------------------------------
    // Constructor
    //--------------------------------------------------------------------------------

    public UIRadarViewModel(IDispatcher dispatcher)
    {
        Targets = RadarSample.LoadTargets(random.NextDouble);

        // 一定間隔でターゲットを入れ替えるデモ駆動
        timer = dispatcher.CreateTimer();
        timer.Interval = TimeSpan.FromSeconds(5);
        Disposables.Add(timer.TickAsObservable().Subscribe(_ => Targets = RadarSample.LoadTargets(random.NextDouble)));
    }

    //--------------------------------------------------------------------------------
    // Navigation
    //--------------------------------------------------------------------------------

    public override Task OnNavigatedToAsync(INavigationContext context)
    {
        timer.Start();
        return Task.CompletedTask;
    }

    public override Task OnNavigatingFromAsync(INavigationContext context)
    {
        timer.Stop();
        return Task.CompletedTask;
    }

    protected override Task OnNotifyBackAsync() => Navigator.ForwardAsync(ViewId.UIMenu2);

    protected override Task OnNotifyFunction1() => OnNotifyBackAsync();
}
#pragma warning restore CA5394
