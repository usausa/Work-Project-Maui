namespace Template.MobileApp.Modules.Navigation.Navigate;

public class NavigateCancelViewModel : AppViewModelBase
{
    private readonly IDialog dialog;

    public NavigateCancelViewModel(
        ApplicationState applicationState,
        IDialog dialog)
        : base(applicationState)
    {
        this.dialog = dialog;
    }

    // ReSharper disable once AsyncVoidMethod
    public override async void OnNavigatedTo(INavigationContext context)
    {
        if (!context.Attribute.IsRestore())
        {
            await Navigator.PostActionAsync(() => BusyState.Using(async () =>
            {
                if (await dialog.ConfirmAsync("Cancel ?", ok: "Yes", cancel: "No"))
                {
                    await Navigator.ForwardAsync(ViewId.NavigationMenu);
                }
            }));
        }
    }

    protected override Task OnNotifyBackAsync() => Navigator.ForwardAsync(ViewId.NavigationMenu);

    protected override Task OnNotifyFunction1() => OnNotifyBackAsync();
}
