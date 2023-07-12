namespace Template.MobileApp.Modules.Pattern;

using Template.MobileApp;

public class PatternListViewModel : AppViewModelBase
{
    public ICommand ForwardCommand { get; }

    public PatternListViewModel(
        ApplicationState applicationState)
        : base(applicationState)
    {
        ForwardCommand = MakeAsyncCommand<ViewId>(x => Navigator.ForwardAsync(x));
    }

    protected override Task OnNotifyBackAsync() => Navigator.ForwardAsync(ViewId.PatternMenu);

    protected override Task OnNotifyFunction1() => OnNotifyBackAsync();
}
