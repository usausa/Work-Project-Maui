namespace Template.MobileApp.Modules.Device;

public sealed partial class DeviceLocationViewModel : AppViewModelBase
{
    private readonly ILocationService locationService;

    [ObservableProperty]
    public partial Location? Location { get; set; }

    public DeviceLocationViewModel(
        ILocationService locationService)
    {
        this.locationService = locationService;

        Disposables.Add(locationService.ObserveLocationChangedOnCurrentContext().Subscribe(x => Location = x.Location));
    }

    protected override Task OnNotifyBackAsync() => Navigator.ForwardAsync(ViewId.DeviceMenu);

    protected override Task OnNotifyFunction1() => OnNotifyBackAsync();

    // ReSharper disable once AsyncVoidMethod
    public override async void OnNavigatedTo(INavigationContext context)
    {
        Location = await locationService.GetLastLocationAsync();

        locationService.Start();
    }

    public override void OnNavigatingFrom(INavigationContext context)
    {
        locationService.Stop();
    }
}
