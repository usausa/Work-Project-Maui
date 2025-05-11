namespace Template.MobileApp.Modules.Device;

public class DeviceStatusViewModel : AppViewModelBase
{
    public ApplicationState ApplicationState { get; }

    public DeviceStatusViewModel(ApplicationState applicationState)
    {
        ApplicationState = applicationState;
    }

    protected override Task OnNotifyBackAsync() => Navigator.ForwardAsync(ViewId.DeviceMenu);

    protected override Task OnNotifyFunction1() => OnNotifyBackAsync();
}
