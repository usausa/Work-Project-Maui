namespace Template.MobileApp.Modules.UI;

public sealed partial class UICockpitViewModel : AppViewModelBase
{
    private readonly IDispatcherTimer timer;

    private double t;

    [ObservableProperty]
    public partial float Pitch { get; set; }

    [ObservableProperty]
    public partial float Roll { get; set; }

    [ObservableProperty]
    public partial float Heading { get; set; }

    [ObservableProperty]
    public partial double Speed { get; set; }

    [ObservableProperty]
    public partial double AltitudeK { get; set; }

    [ObservableProperty]
    public partial double Rpm { get; set; }

    public UICockpitViewModel(IDispatcher dispatcher)
    {
        // 正弦波でゆらぎを与えるデモ駆動
        timer = dispatcher.CreateTimer();
        timer.Interval = TimeSpan.FromMilliseconds(50);
        Disposables.Add(timer.TickAsObservable().Subscribe(_ => Update()));
        Update();
    }

    private void Update()
    {
        t += 0.05;

        Pitch = (float)((6.5 * Math.Sin(t * 0.35)) + (2.5 * Math.Sin(t * 0.9)));
        Roll = (float)((16.0 * Math.Sin(t * 0.22)) + (5.0 * Math.Sin(t * 0.7)));
        Heading = (float)(((((t * 6.0) + (20.0 * Math.Sin(t * 0.12))) % 360.0) + 360.0) % 360.0);
        Speed = 260.0 + (45.0 * Math.Sin(t * 0.18)) + (8.0 * Math.Sin(t * 0.6));
        AltitudeK = 8.5 + (1.8 * Math.Sin(t * 0.12));
        Rpm = 78.0 + (12.0 * Math.Sin(t * 0.4));
    }

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

    protected override Task OnNotifyBackAsync() => Navigator.ForwardAsync(ViewId.UIMenu);

    protected override Task OnNotifyFunction1() => OnNotifyBackAsync();
}
