namespace Template.MobileApp.Modules.UI;

using Template.MobileApp.Graphics;

public sealed class UIFlightHudViewModel : AppViewModelBase
{
    public FlightHudScene Scene { get; } = new();

    public UIFlightHudViewModel()
    {
        Disposables.Add(Scene);
    }

    public override Task OnNavigatedToAsync(INavigationContext context)
    {
        Scene.Start();
        return Task.CompletedTask;
    }

    public override Task OnNavigatingFromAsync(INavigationContext context)
    {
        Scene.Stop();
        return Task.CompletedTask;
    }

    protected override Task OnNotifyBackAsync() => Navigator.ForwardAsync(ViewId.UIMenu);

    protected override Task OnNotifyFunction1() => OnNotifyBackAsync();
}
