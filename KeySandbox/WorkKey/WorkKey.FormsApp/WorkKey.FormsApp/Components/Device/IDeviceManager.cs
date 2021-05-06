namespace WorkKey.FormsApp.Components.Device
{
    using System;

    public interface IDeviceManager
    {
        IObservable<NetworkState> NetworkState { get; }

        NetworkState GetNetworkState();

        string GetVersion();
    }
}
