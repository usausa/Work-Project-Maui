namespace Business.FormsApp.Components.Device
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reactive.Subjects;

    using Xamarin.Essentials;

    public abstract class DeviceManagerBase : IDeviceManager
    {
        private readonly BehaviorSubject<bool> keyboardState = new BehaviorSubject<bool>(false);

        private readonly Subject<bool> screenState = new Subject<bool>();

        private readonly BehaviorSubject<double> batteryLevel;

        private readonly BehaviorSubject<bool> wifiConnected;

        private bool lastKeyboardState;

        public IObservable<bool> KeyboardState => keyboardState;

        public IObservable<bool> ScreenState => screenState;

        public IObservable<double> BatteryLevel => batteryLevel;

        public IObservable<bool> WifiConnected => wifiConnected;

        protected DeviceManagerBase()
        {
            batteryLevel = new BehaviorSubject<double>(Battery.ChargeLevel);
            Battery.BatteryInfoChanged += (sender, args) =>
            {
                Console.WriteLine($"BatteryInfoChanged: Level=[{args.ChargeLevel}], State=[{args.State}], Source=[{args.PowerSource}]");
                batteryLevel.OnNext(Battery.ChargeLevel);
            };

            wifiConnected = new BehaviorSubject<bool>(IsWifiConnected(Connectivity.ConnectionProfiles));
            Connectivity.ConnectivityChanged += (sender, args) =>
            {
                var connected = IsWifiConnected(Connectivity.ConnectionProfiles);
                Console.WriteLine($"ConnectivityChanged: WiFi=[{connected}]");
                wifiConnected.OnNext(connected);
            };
        }

        private static bool IsWifiConnected(IEnumerable<ConnectionProfile> profiles)
        {
            return profiles.Contains(ConnectionProfile.WiFi);
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
    }
}
