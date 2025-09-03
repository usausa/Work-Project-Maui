namespace WorkNavigation.Modules.Navigation;

public sealed class NavigationMenuViewModel : AppViewModelBase
{
    public IObserveCommand ForwardCommand { get; }

    public NavigationMenuViewModel()
    {
        ForwardCommand = MakeAsyncCommand<ViewId>(x => Navigator.ForwardAsync(x));
    }
}
