namespace Baseline.FormsApp.Components.Device
{
    using System;
    using System.Reactive.Subjects;

    using Xamarin.Essentials;

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Ignore")]
    public abstract class DeviceManagerBase : IDeviceManager
    {
        private readonly BehaviorSubject<bool> keyboardState = new BehaviorSubject<bool>(false);

        private readonly Subject<bool> screenState = new Subject<bool>();

        private readonly BehaviorSubject<BatteryInformation> batteryInformation;

        private readonly BehaviorSubject<bool> networkConnected;

        private bool lastKeyboardState;

        public IObservable<bool> KeyboardState => keyboardState;

        public IObservable<bool> ScreenState => screenState;

        public IObservable<BatteryInformation> BatteryInformation => batteryInformation;

        public IObservable<bool> NetworkConnected => networkConnected;

        protected DeviceManagerBase()
        {
            batteryInformation = new BehaviorSubject<BatteryInformation>(ToBatteryInformation(Battery.State, Battery.ChargeLevel));
            Battery.BatteryInfoChanged += (sender, args) =>
            {
                System.Diagnostics.Debug.WriteLine($"BatteryInfoChanged: Level=[{args.ChargeLevel}], State=[{args.State}], Source=[{args.PowerSource}]");
                batteryInformation.OnNext(ToBatteryInformation(args.State, args.ChargeLevel));
            };

            networkConnected = new BehaviorSubject<bool>(IsNetworkConnected(Connectivity.NetworkAccess));
            Connectivity.ConnectivityChanged += (sender, args) =>
            {
                networkConnected.OnNext(IsNetworkConnected(args.NetworkAccess));
            };
        }

        private static BatteryInformation ToBatteryInformation(BatteryState batteryState, double chargeLevel)
        {
            return new BatteryInformation(
                batteryState == BatteryState.Full ? BatteryStatus.Full : batteryState == BatteryState.Charging ? BatteryStatus.Charging : BatteryStatus.None,
                (int)(chargeLevel * 100));
        }

        private static bool IsNetworkConnected(NetworkAccess access)
        {
            return access != NetworkAccess.None && access != NetworkAccess.Unknown;
        }

        protected void RaiseKeyboardState(bool visible)
        {
            if (lastKeyboardState != visible)
            {
                lastKeyboardState = visible;
                keyboardState.OnNext(visible);
            }
        }

        protected void RaiseScreenState(bool on)
        {
            screenState.OnNext(on);
        }

        public abstract void MoveTaskToBack();
    }
}
