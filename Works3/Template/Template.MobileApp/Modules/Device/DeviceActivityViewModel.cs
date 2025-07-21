namespace Template.MobileApp.Modules.Device;

using Template.MobileApp.Components;

public sealed partial class DeviceActivityViewModel : AppViewModelBase
{
    private readonly IActivityRecognizer activityRecognizer;

    [ObservableProperty]
    public partial int Count { get; set; }

    public DeviceActivityViewModel(IActivityRecognizer activityRecognizer)
    {
        this.activityRecognizer = activityRecognizer;

        Disposables.Add(activityRecognizer.ChangedAsObservable().ObserveOnCurrentContext().Subscribe(x =>
        {
            Count = x.Counter;
        }));
    }

    public override Task OnNavigatedToAsync(INavigationContext context)
    {
        activityRecognizer.Enabled = true;
        return Task.CompletedTask;
    }

    public override Task OnNavigatingFromAsync(INavigationContext context)
    {
        activityRecognizer.Enabled = false;
        return Task.CompletedTask;
    }

    protected override Task OnNotifyBackAsync() => Navigator.ForwardAsync(ViewId.DeviceMenu);

    protected override Task OnNotifyFunction1() => OnNotifyBackAsync();
}
