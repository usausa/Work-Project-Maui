namespace Business.FormsApp
{
    using System;

    using Business.FormsApp.Components.Keyboard;
    using Business.FormsApp.Views;

    using Smart.ComponentModel;
    using Smart.Forms.Input;
    using Smart.Forms.ViewModels;
    using Smart.Navigation;

    public class MainPageViewModel : ViewModelBase, IShellControl
    {
        public NotificationValue<string> Title { get; } = new NotificationValue<string>();

        public NotificationValue<bool> CanBack { get; } = new NotificationValue<bool>();

        public NotificationValue<string> Function1Text { get; } = new NotificationValue<string>();

        public NotificationValue<string> Function2Text { get; } = new NotificationValue<string>();

        public NotificationValue<string> Function3Text { get; } = new NotificationValue<string>();

        public NotificationValue<string> Function4Text { get; } = new NotificationValue<string>();

        public NotificationValue<bool> Function1Enabled { get; } = new NotificationValue<bool>();

        public NotificationValue<bool> Function2Enabled { get; } = new NotificationValue<bool>();

        public NotificationValue<bool> Function3Enabled { get; } = new NotificationValue<bool>();

        public NotificationValue<bool> Function4Enabled { get; } = new NotificationValue<bool>();

        public ApplicationState ApplicationState { get; }

        public INavigator Navigator { get; }

        public AsyncCommand BackCommand { get; }

        public AsyncCommand Function1Command { get; }

        public AsyncCommand Function2Command { get; }

        public AsyncCommand Function3Command { get; }

        public AsyncCommand Function4Command { get; }

        public MainPageViewModel(
            ApplicationState applicationState,
            INavigator navigator,
            IKeyboardManager keyboardManager)
            : base(applicationState)
        {
            ApplicationState = applicationState;
            Navigator = navigator;

            Disposables.Add(keyboardManager.StateChanged.
                Subscribe(x =>
                {
                    System.Diagnostics.Debug.WriteLine("keyboard" + x);
                    applicationState.KeyboardVisible = x;
                }));

            BackCommand = MakeAsyncCommand(
                    () => Navigator.NotifyAsync(ShellKeys.Back),
                    () => CanBack.Value)
                .Observe(CanBack);
            Function1Command = MakeAsyncCommand(
                    () => Navigator.NotifyAsync(ShellKeys.Function1),
                    () => Function1Enabled.Value)
                .Observe(Function1Enabled);
            Function2Command = MakeAsyncCommand(
                    () => Navigator.NotifyAsync(ShellKeys.Function2),
                    () => Function2Enabled.Value)
                .Observe(Function2Enabled);
            Function3Command = MakeAsyncCommand(
                    () => Navigator.NotifyAsync(ShellKeys.Function3),
                    () => Function3Enabled.Value)
                .Observe(Function3Enabled);
            Function4Command = MakeAsyncCommand(
                    () => Navigator.NotifyAsync(ShellKeys.Function4),
                    () => Function4Enabled.Value)
                .Observe(Function4Enabled);
        }
    }
}
