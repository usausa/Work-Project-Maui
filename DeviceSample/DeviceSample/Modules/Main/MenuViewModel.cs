namespace DeviceSample.Modules.Main;

public class MenuViewModel : AppViewModelBase
{
    public NotificationValue<Version> Version { get; } = new();

    public ICommand ForwardCommand { get; }

    public MenuViewModel(
        ApplicationState applicationState,
        IAppInfo appInfo)
        : base(applicationState)
    {
        Version.Value = appInfo.Version;

        ForwardCommand = MakeAsyncCommand<ViewId>(x => Navigator.ForwardAsync(x));
    }
}
