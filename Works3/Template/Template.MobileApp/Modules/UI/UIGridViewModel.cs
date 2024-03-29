namespace Template.MobileApp.Modules.UI;

public class UIGridViewModel : AppViewModelBase
{
    public UIGridViewModel(
        ApplicationState applicationState)
        : base(applicationState)
    {
    }

    protected override Task OnNotifyBackAsync() => Navigator.ForwardAsync(ViewId.UIMenu);

    protected override Task OnNotifyFunction1() => OnNotifyBackAsync();
}
