namespace WorkKey.FormsApp.Components.Device
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reactive.Subjects;
    using Smart;
    using WorkKey.FormsApp.Shell;
    using Xamarin.Essentials;

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Ignore")]
    public abstract class DeviceManagerBase : IDeviceManager
    {
        public event EventHandler<EventArgs<ShellEvent>> ShellKeyDown;

        private readonly BehaviorSubject<NetworkState> networkState;

        public IObservable<NetworkState> NetworkState => networkState;

        protected DeviceManagerBase()
        {
            networkState = new BehaviorSubject<NetworkState>(GetNetworkState(Connectivity.NetworkAccess, Connectivity.ConnectionProfiles));
            Connectivity.ConnectivityChanged += (_, args) =>
            {
                networkState.OnNext(GetNetworkState(args.NetworkAccess, args.ConnectionProfiles));
            };
        }

        protected void RaiseShellKeyDown(ShellEvent ev)
        {
            ShellKeyDown?.Invoke(this, new EventArgs<ShellEvent>(ev));
        }

        private static NetworkState GetNetworkState(NetworkAccess access, IEnumerable<ConnectionProfile> profiles)
        {
            if (access != NetworkAccess.None && access != NetworkAccess.Unknown)
            {
                return profiles.Any(x => x == ConnectionProfile.Ethernet || x == ConnectionProfile.WiFi)
                    ? WorkKey.FormsApp.Components.Device.NetworkState.ConnectedHighSpeed
                    : WorkKey.FormsApp.Components.Device.NetworkState.Connected;
            }

            return WorkKey.FormsApp.Components.Device.NetworkState.Disconnected;
        }

        public NetworkState GetNetworkState() => GetNetworkState(Connectivity.NetworkAccess, Connectivity.ConnectionProfiles);

        public abstract string GetVersion();
    }
}
