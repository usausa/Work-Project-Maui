namespace Baseline.FormsApp
{
    using Baseline.FormsApp.Components.Device;

    using Smart.ComponentModel;
    using Smart.Forms.ViewModels;

    public sealed class ApplicationState : NotificationObject, IBusyState
    {
        private bool isBusy;

        private bool keyboardVisible;

        private BatteryInformation batteryInformation;

        private bool networkConnected;

        public bool IsBusy
        {
            get => isBusy;
            set => SetProperty(ref isBusy, value);
        }

        public bool KeyboardVisible
        {
            get => keyboardVisible;
            set => SetProperty(ref keyboardVisible, value);
        }

        public BatteryInformation BatteryInformation
        {
            get => batteryInformation;
            set => SetProperty(ref batteryInformation, value);
        }

        public bool NetworkConnected
        {
            get => networkConnected;
            set => SetProperty(ref networkConnected, value);
        }
    }
}
