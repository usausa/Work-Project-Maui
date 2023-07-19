namespace Template.MobileApp.Modules.Network;

using Template.MobileApp;
using Template.MobileApp.State;

public class NetworkViewModel : AppViewModelBase
{
    private readonly Settings settings;

    private readonly IDialog dialog;

    public NetworkViewModel(
        ApplicationState applicationState,
        Settings settings,
        IDialog dialog)
        : base(applicationState)
    {
        this.settings = settings;
        this.dialog = dialog;
    }

    protected override Task OnNotifyBackAsync() => Navigator.ForwardAsync(ViewId.Menu);

    protected override Task OnNotifyFunction1() => OnNotifyBackAsync();

    public override async void OnNavigatedTo(INavigationContext context)
    {
        if (String.IsNullOrEmpty(settings.ApiEndPoint))
        {
            await Navigator.PostActionAsync(() => BusyState.Using(async () =>
            {
                await dialog.InformationAsync("API EndPoint not configured.");

                await Navigator.ForwardAsync(ViewId.Menu);
            }));
        }
    }
}
