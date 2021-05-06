namespace WorkKey.FormsApp
{
    using System;
    using System.Reactive.Linq;
    using System.Threading;
    using System.Windows.Input;

    using Smart;
    using Smart.ComponentModel;
    using Smart.Forms.ViewModels;
    using Smart.Navigation;

    using WorkKey.FormsApp.Components.Device;
    using WorkKey.FormsApp.Shell;

    public class MainPageViewModel : ViewModelBase, IShellControl
    {
        public ApplicationState ApplicationState { get; }

        public INavigator Navigator { get; }

        public NotificationValue<string> Title { get; } = new();

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
            IDeviceManager deviceManager)
            : base(applicationState)
        {
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

            var functionEnables = new[] { Function1Enabled, Function2Enabled, Function3Enabled, Function4Enabled };
            Disposables.Add(Observable
                .FromEvent<EventHandler<EventArgs<ShellEvent>>, EventArgs<ShellEvent>>(h => (_, e) => h(e), h => deviceManager.ShellKeyDown += h, h => deviceManager.ShellKeyDown -= h)
                .ObserveOn(SynchronizationContext.Current)
                .Subscribe(e =>
                {
                    if (BusyState.IsBusy || !functionEnables[e.Data - ShellEvent.Function1].Value)
                    {
                        return;
                    }

                    Navigator.NotifyAsync(e.Data);
                }));
        }
    }
}
