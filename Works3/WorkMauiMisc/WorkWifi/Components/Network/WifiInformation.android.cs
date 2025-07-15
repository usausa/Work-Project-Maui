namespace WorkWifi.Components.Network;

using Android.Content;
using Android.Net;
using Android.Net.Wifi;

using Java.Net;

internal partial class WifiInformationImplementation
{
    private static ConnectivityManager? connectivityManager;

    private WifiInformationCallBack? callback;

    private static ConnectivityManager ConnectivityManager =>
        connectivityManager ??= (ConnectivityManager)Android.App.Application.Context.GetSystemService(Context.ConnectivityService)!;

    private partial void StartListener()
    {
        if (callback is not null)
        {
            return;
        }

        var request = new NetworkRequest.Builder()
            .AddTransportType(TransportType.Wifi)!
            .Build()!;
        callback = new WifiInformationCallBack(this);
        ConnectivityManager.RegisterNetworkCallback(request, callback);
    }

    private partial void StopListener()
    {
        if (callback is null)
        {
            return;
        }

        ConnectivityManager.UnregisterNetworkCallback(callback);
        callback.Dispose();
        callback = null;
    }

    private class WifiInformationCallBack : ConnectivityManager.NetworkCallback
    {
        private readonly WifiInformationImplementation parent;

        public WifiInformationCallBack(WifiInformationImplementation parent)
            : base((int)NetworkCallbackFlags.IncludeLocationInfo)
        {
            this.parent = parent;
        }

        public override void OnAvailable(Network network)
        {
            parent.RaiseStateChanged(true);
        }

        public override void OnLost(Network network)
        {
            parent.RaiseStateChanged(false);
        }

        public override void OnCapabilitiesChanged(Network network, NetworkCapabilities networkCapabilities)
        {
            var wifiInfo = (WifiInfo)networkCapabilities.TransportInfo!;
#pragma warning disable CS0618
            parent.RaiseCapabilityChanged(wifiInfo.SSID ?? string.Empty, WifiManager.CalculateSignalLevel(networkCapabilities.SignalStrength, 100));
#pragma warning restore CS0618
        }

        public override void OnLinkPropertiesChanged(Network network, LinkProperties linkProperties)
        {
            var list = new List<LinkAddress>();
            foreach (var linkAddress in linkProperties.LinkAddresses)
            {
                if (linkAddress.Address is Inet4Address ip4)
                {
                    list.Add(new LinkAddress(ip4.HostAddress!, true));
                }
                else if (linkAddress.Address is Inet6Address ip6)
                {
                    list.Add(new LinkAddress(ip6.HostAddress!, false));
                }
            }

            parent.RaiseLingChanged(list);
        }
    }
}