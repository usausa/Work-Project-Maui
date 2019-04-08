namespace Baseline.FormsApp
{
    using System;

    using Baseline.FormsApp.Components.Device;
    using Baseline.FormsApp.Shell;

    using Smart.ComponentModel;
    using Smart.Forms.ViewModels;
    using Smart.Navigation;

    public class MainPageViewModel : ViewModelBase, IShellControl
    {
        public static MainPageViewModel DesignInstance { get; } = null; // For design

        public NotificationValue<string> Title { get; } = new NotificationValue<string>();

        public ApplicationState ApplicationState { get; }

        public INavigator Navigator { get; }

        public MainPageViewModel(
            ApplicationState applicationState,
            INavigator navigator,
            IDeviceManager deviceManager)
            : base(applicationState)
        {
            ApplicationState = applicationState;
            Navigator = navigator;

            Disposables.Add(deviceManager.ScreenState.
                Subscribe(x =>
                {
                }));
            Disposables.Add(deviceManager.KeyboardState.
                Subscribe(x => applicationState.KeyboardVisible = x));
            Disposables.Add(deviceManager.BatteryInformation.
                Subscribe(x => applicationState.BatteryInformation = x));
            Disposables.Add(deviceManager.NetworkConnected.
                Subscribe(x => applicationState.NetworkConnected = x));
        }
    }
}
