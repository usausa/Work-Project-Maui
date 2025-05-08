namespace Template.MobileApp;

using Template.MobileApp.Shell;

[ObservableGeneratorOption(Reactive = true, ViewModel = true)]
public partial class MainPageViewModel : ExtendViewModelBase, IShellControl, IAppLifecycle
{
    private readonly IScreen screen;

    public ApplicationState ApplicationState { get; }

    public INavigator Navigator { get; }

    [ObservableProperty]
    public partial string Title { get; set; } = default!;

    [ObservableProperty]
    public partial bool HeaderVisible { get; set; }

    [ObservableProperty]
    public partial bool FunctionVisible { get; set; }

    [ObservableProperty]
    public partial string Function1Text { get; set; } = default!;
    [ObservableProperty]
    public partial string Function2Text { get; set; } = default!;
    [ObservableProperty]
    public partial string Function3Text { get; set; } = default!;
    [ObservableProperty]
    public partial string Function4Text { get; set; } = default!;

    [ObservableProperty]
    public partial bool Function1Enabled { get; set; }
    [ObservableProperty]
    public partial bool Function2Enabled { get; set; }
    [ObservableProperty]
    public partial bool Function3Enabled { get; set; }
    [ObservableProperty]
    public partial bool Function4Enabled { get; set; }

    public ICommand Function1Command { get; }
    public ICommand Function2Command { get; }
    public ICommand Function3Command { get; }
    public ICommand Function4Command { get; }

    //--------------------------------------------------------------------------------
    // Constructor
    //--------------------------------------------------------------------------------

    public MainPageViewModel(
        ApplicationState applicationState,
        ILogger<MainPageViewModel> log,
        INavigator navigator,
        IScreen screen,
        IDialog dialog)
        : base(applicationState)
    {
        this.screen = screen;
        ApplicationState = applicationState;
        Navigator = navigator;

        Function1Command = MakeAsyncCommand(
                () => Navigator.NotifyAsync(ShellEvent.Function1),
                () => Function1Enabled)
            .Observe(this, nameof(Function1Enabled));
        Function2Command = MakeAsyncCommand(
                () => Navigator.NotifyAsync(ShellEvent.Function2),
                () => Function2Enabled)
            .Observe(this, nameof(Function2Enabled));
        Function3Command = MakeAsyncCommand(
                () => Navigator.NotifyAsync(ShellEvent.Function3),
                () => Function3Enabled)
            .Observe(this, nameof(Function3Enabled));
        Function4Command = MakeAsyncCommand(
                () => Navigator.NotifyAsync(ShellEvent.Function4),
                () => Function4Enabled)
            .Observe(this, nameof(Function4Enabled));

        // Screen lock detection
        // ReSharper disable AsyncVoidLambda
        Disposables.Add(Observable
            .FromEvent<EventHandler<ScreenStateEventArgs>, ScreenStateEventArgs>(static h => (_, e) => h(e), h => screen.ScreenStateChanged += h, h => screen.ScreenStateChanged -= h)
            .ObserveOn(SynchronizationContext.Current!)
            .Subscribe(async x =>
            {
                log.DebugScreenStateChanged(x.ScreenOn);
                if (x.ScreenOn)
                {
                    await dialog.Toast("Screen on", true);
                }
            }));
        // ReSharper restore AsyncVoidLambda
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
        // TODO Device activate
    }

    public void OnDeactivated()
    {
        // TODO Device deactivate
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
