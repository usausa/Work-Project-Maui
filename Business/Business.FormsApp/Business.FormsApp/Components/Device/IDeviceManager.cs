namespace Business.FormsApp.Components.Device
{
    using System;

    public interface IDeviceManager
    {
        IObservable<bool> KeyboardState { get; }

        IObservable<bool> ScreenState { get; }
    }
}
