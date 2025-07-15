namespace WorkWifi.Components.Network;

// [MEMO] 接続状態はConnectivity併用？

public sealed class WifiStateEventArgs : EventArgs
{
    public bool Active { get; }

    public WifiStateEventArgs(bool active)
    {
        Active = active;
    }

    public override string ToString() => $"{nameof(Active)}: {Active}";
}

public sealed class WifiCapabilityEventArgs : EventArgs
{
    public string Ssid { get; }

    public int SignalLevel { get; }

    public WifiCapabilityEventArgs(string ssid, int signalLevel)
    {
        Ssid = ssid;
        SignalLevel = signalLevel;
    }

    public override string ToString() => $"{nameof(Ssid)}: {Ssid}, {nameof(SignalLevel)}: {SignalLevel}";
}

public sealed class LinkAddress
{
    public string Address { get; }

    public bool IsIPv4 { get; }

    public bool IsIPv6 => !IsIPv4;

    public LinkAddress(string address, bool isIPv4)
    {
        Address = address;
        IsIPv4 = isIPv4;
    }
}

public sealed class WifiLinkEventArgs : EventArgs
{
    public IReadOnlyList<LinkAddress> Addresses { get; }

    public WifiLinkEventArgs(IReadOnlyList<LinkAddress> addresses)
    {
        Addresses = addresses;
    }

    public override string ToString() => $"{nameof(Addresses)}: {String.Join(", ", Addresses)}";
}

public interface IWifiInformation
{
    event EventHandler<WifiStateEventArgs> StateChanged;

    event EventHandler<WifiCapabilityEventArgs> CapabilityChanged;

    event EventHandler<WifiLinkEventArgs> LinkChanged;
}

public static class WifiInformation
{
    private static WifiInformationImplementation? current;

    public static IWifiInformation Current => current ??= new WifiInformationImplementation();

    public static event EventHandler<WifiStateEventArgs> StateChanged
    {
        add => Current.StateChanged += value;
        remove => Current.StateChanged -= value;
    }

    public static event EventHandler<WifiCapabilityEventArgs> CapabilityChanged
    {
        add => Current.CapabilityChanged += value;
        remove => Current.CapabilityChanged -= value;
    }

    public static event EventHandler<WifiLinkEventArgs> LinkChanged
    {
        add => Current.LinkChanged += value;
        remove => Current.LinkChanged -= value;
    }
}

internal partial class WifiInformationImplementation : IWifiInformation
{
    private event EventHandler<WifiStateEventArgs>? StateChangedInternal;

    private event EventHandler<WifiCapabilityEventArgs>? CapabilityChangedInternal;

    private event EventHandler<WifiLinkEventArgs>? LinkChangedInternal;

    public event EventHandler<WifiStateEventArgs> StateChanged
    {
        add
        {
            StartListenerIfNeed();
            StateChangedInternal += value;
        }
        remove
        {
            StateChangedInternal -= value;
            StopListenerIfNeed();
        }
    }

    public event EventHandler<WifiCapabilityEventArgs> CapabilityChanged
    {
        add
        {
            StartListenerIfNeed();
            CapabilityChangedInternal += value;
        }
        remove
        {
            CapabilityChangedInternal -= value;
            StopListenerIfNeed();
        }
    }

    public event EventHandler<WifiLinkEventArgs> LinkChanged
    {
        add
        {
            StartListenerIfNeed();
            LinkChangedInternal += value;
        }
        remove
        {
            LinkChangedInternal -= value;
            StopListenerIfNeed();
        }
    }

    private void StartListenerIfNeed()
    {
        if ((StateChangedInternal is null) && (CapabilityChangedInternal is null) && (LinkChangedInternal is null))
        {
            StartListener();
        }
    }

    private void StopListenerIfNeed()
    {
        if ((StateChangedInternal is null) && (CapabilityChangedInternal is null) && (LinkChangedInternal is null))
        {
            StopListener();
        }
    }

    private void RaiseStateChanged(bool active)
    {
        StateChangedInternal?.Invoke(null, new WifiStateEventArgs(active));
    }

    private void RaiseCapabilityChanged(string ssid, int signalLevel)
    {
        CapabilityChangedInternal?.Invoke(null, new WifiCapabilityEventArgs(ssid, signalLevel));
    }

    private void RaiseLingChanged(IReadOnlyList<LinkAddress> addresses)
    {
        LinkChangedInternal?.Invoke(null, new WifiLinkEventArgs(addresses));
    }

    private partial void StartListener();

    private partial void StopListener();
}
