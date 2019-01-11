namespace Business.FormsApp.Components.Device
{
    using System;

    public interface IDeviceManager
    {
        IObservable<bool> KeyboardState { get; }

        IObservable<bool> ScreenState { get; }

        IObservable<double> BatteryLevel { get; }

        IObservable<bool> WifiConnected { get; }
    }
}
