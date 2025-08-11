namespace Template.MobileApp.Modules.UI;

public sealed class UISocialViewModel : AppViewModelBase
{
    private readonly IScreen screen;

    public UISocialViewModel(
        IScreen screen)
    {
        this.screen = screen;
    }

    public override Task OnNavigatedToAsync(INavigationContext context)
    {
        screen.SetFullscreen(true);
        return Task.CompletedTask;
    }

    public override Task OnNavigatingFromAsync(INavigationContext context)
    {
        screen.SetFullscreen(false);
        return Task.CompletedTask;
    }

    protected override Task OnNotifyBackAsync() => Navigator.ForwardAsync(ViewId.UIMenu);

    protected override Task OnNotifyFunction1() => OnNotifyBackAsync();
}
