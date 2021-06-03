namespace WorkKey.FormsApp.Components.Device
{
    using System;
    using Smart;
    using WorkKey.FormsApp.Shell;

    public interface IDeviceManager
    {
        public event EventHandler<EventArgs<ShellEvent>> ShellKeyDown;

        IObservable<NetworkState> NetworkState { get; }

        NetworkState GetNetworkState();

        string GetVersion();
    }
}
