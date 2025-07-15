namespace WorkWifi;

using WorkWifi.Components.Network;

using Smart.ComponentModel;
using Smart.Maui.ViewModels;

public class MainPageViewModel : ExtendViewModelBase
{
    public NotificationValue<string?> BatteryLevel { get; } = new();

    public NotificationValue<string?> BatteryState { get; } = new();

    public NotificationValue<string?> BatterySource { get; } = new();

    public NotificationValue<string?> Connection { get; } = new();

    public NotificationValue<string?> WifiActive { get; } = new();

    public NotificationValue<string?> WifiSsid { get; } = new();

    public NotificationValue<string?> WifiLevel { get; } = new();

    public NotificationValue<string?> WifiAddress { get; } = new();

    public MainPageViewModel()
    {
        Battery.BatteryInfoChanged += BatteryOnBatteryInfoChanged;
        Connectivity.ConnectivityChanged += ConnectivityOnConnectivityChanged;
        Connection.Value = String.Join(", ", Connectivity.ConnectionProfiles);

        WifiInformation.StateChanged += WifiInformationOnStateChanged;
        WifiInformation.CapabilityChanged += WifiInformationOnCapabilityChanged;
        WifiInformation.LinkChanged += WifiInformationOnLinkChanged;
    }

    private void BatteryOnBatteryInfoChanged(object? sender, BatteryInfoChangedEventArgs e)
    {
        BatteryLevel.Value = $"{(int)(e.ChargeLevel * 100)}";
        BatteryState.Value = e.State.ToString();
        BatterySource.Value = e.PowerSource.ToString();
    }

    private void ConnectivityOnConnectivityChanged(object? sender, ConnectivityChangedEventArgs e)
    {
        Connection.Value = String.Join(", ", e.ConnectionProfiles);
    }

    private void WifiInformationOnStateChanged(object? sender, WifiStateEventArgs e)
    {
        WifiActive.Value = e.Active.ToString();
    }

    private void WifiInformationOnCapabilityChanged(object? sender, WifiCapabilityEventArgs e)
    {
        WifiSsid.Value = e.Ssid.Trim('"');
        WifiLevel.Value = $"{e.SignalLevel}";
    }

    private void WifiInformationOnLinkChanged(object? sender, WifiLinkEventArgs e)
    {
        var ip4 = e.Addresses.FirstOrDefault(x => x.IsIPv4);
        WifiAddress.Value = ip4?.Address;
    }
}
