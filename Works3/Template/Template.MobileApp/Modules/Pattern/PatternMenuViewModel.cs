namespace Template.MobileApp.Modules.Pattern;

public class PatternMenuViewModel : AppViewModelBase
{
    public ICommand ForwardCommand { get; }

    public PatternMenuViewModel(
        ApplicationState applicationState)
        : base(applicationState)
    {
        ForwardCommand = MakeAsyncCommand<ViewId>(x => Navigator.ForwardAsync(x));
    }

    protected override Task OnNotifyBackAsync() => Navigator.ForwardAsync(ViewId.Menu);

    protected override Task OnNotifyFunction1() => OnNotifyBackAsync();
}
