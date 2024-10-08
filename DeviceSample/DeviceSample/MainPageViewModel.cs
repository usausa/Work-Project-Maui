namespace DeviceSample;

using DeviceSample.Shell;

public class MainPageViewModel : ViewModelBase, IShellControl, IAppLifecycle
{
    private readonly IScreen screen;

    public ApplicationState ApplicationState { get; }

    public INavigator Navigator { get; }

    public NotificationValue<string> Title { get; } = new();

    public NotificationValue<bool> HeaderVisible { get; } = new();

    public NotificationValue<bool> FunctionVisible { get; } = new();

    public NotificationValue<string> Function1Text { get; } = new();
    public NotificationValue<string> Function2Text { get; } = new();
    public NotificationValue<string> Function3Text { get; } = new();
    public NotificationValue<string> Function4Text { get; } = new();

    public NotificationValue<bool> Function1Enabled { get; } = new();
    public NotificationValue<bool> Function2Enabled { get; } = new();
    public NotificationValue<bool> Function3Enabled { get; } = new();
    public NotificationValue<bool> Function4Enabled { get; } = new();

    public ICommand Function1Command { get; }
    public ICommand Function2Command { get; }
    public ICommand Function3Command { get; }
    public ICommand Function4Command { get; }

    //--------------------------------------------------------------------------------
    // Constructor
    //--------------------------------------------------------------------------------

    public MainPageViewModel(
        ApplicationState applicationState,
        INavigator navigator,
        IScreen screen)
        : base(applicationState)
    {
        this.screen = screen;
        ApplicationState = applicationState;
        Navigator = navigator;

        Function1Command = MakeAsyncCommand(
                () => Navigator.NotifyAsync(ShellEvent.Function1),
                () => Function1Enabled.Value)
            .Observe(Function1Enabled);
        Function2Command = MakeAsyncCommand(
                () => Navigator.NotifyAsync(ShellEvent.Function2),
                () => Function2Enabled.Value)
            .Observe(Function2Enabled);
        Function3Command = MakeAsyncCommand(
                () => Navigator.NotifyAsync(ShellEvent.Function3),
                () => Function3Enabled.Value)
            .Observe(Function3Enabled);
        Function4Command = MakeAsyncCommand(
                () => Navigator.NotifyAsync(ShellEvent.Function4),
                () => Function4Enabled.Value)
            .Observe(Function4Enabled);
    }

    //--------------------------------------------------------------------------------
    // Lifecycle
    //--------------------------------------------------------------------------------

    public void OnCreated()
    {
        screen.EnableDetectScreenState(true);
    }

    public void OnActivated()
    {
    }

    public void OnDeactivated()
    {
    }

    public void OnStopped()
    {
    }

    public void OnResumed()
    {
    }

    public void OnDestroying()
    {
    }
}
