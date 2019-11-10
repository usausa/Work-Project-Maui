namespace Baseline.FormsApp.Components.Device
{
    using System;

    public interface IDeviceManager
    {
        IObservable<bool> KeyboardState { get; }

        IObservable<bool> ScreenState { get; }

        IObservable<BatteryInformation> BatteryInformation { get; }

        IObservable<bool> NetworkConnected { get; }

        void MoveTaskToBack();
    }
}
