namespace Template.MobileApp.Components.Device;

public interface IDeviceManager
{
    // Status

    IObservable<NetworkState> NetworkState { get; }

    NetworkState GetNetworkState();

    // Display

    Orientation GetOrientation();

    void SetOrientation(Orientation orientation);

    ValueTask<Stream> TakeScreenshotAsync();

    void KeepScreenOn(bool value);
}

public sealed partial class DeviceManager : IDeviceManager, IDisposable
{
    private readonly IDeviceDisplay deviceDisplay;

    private readonly IScreenshot screenshot;

    private readonly BehaviorSubject<NetworkState> networkState;

    public IObservable<NetworkState> NetworkState => networkState;

    public DeviceManager(
        IDeviceDisplay deviceDisplay,
        IScreenshot screenshot)
    {
        this.deviceDisplay = deviceDisplay;
        this.screenshot = screenshot;

        networkState = new BehaviorSubject<NetworkState>(GetNetworkState(Connectivity.NetworkAccess, Connectivity.ConnectionProfiles));
        Connectivity.ConnectivityChanged += (_, args) =>
        {
            networkState.OnNext(GetNetworkState(args.NetworkAccess, args.ConnectionProfiles));
        };
    }

    public void Dispose()
    {
        networkState.Dispose();
    }

    // ------------------------------------------------------------
    // Status
    // ------------------------------------------------------------

    private static NetworkState GetNetworkState(NetworkAccess access, IEnumerable<ConnectionProfile> profiles)
    {
        if (access != NetworkAccess.None && access != NetworkAccess.Unknown)
        {
            return profiles.Any(x => x == ConnectionProfile.Ethernet || x == ConnectionProfile.WiFi)
                ? Template.MobileApp.Components.Device.NetworkState.ConnectedHighSpeed
                : Template.MobileApp.Components.Device.NetworkState.Connected;
        }

        return Template.MobileApp.Components.Device.NetworkState.Disconnected;
    }

    public NetworkState GetNetworkState() => GetNetworkState(Connectivity.NetworkAccess, Connectivity.ConnectionProfiles);

    // ------------------------------------------------------------
    // Display
    // ------------------------------------------------------------

    public Orientation GetOrientation() =>
        deviceDisplay.MainDisplayInfo.Orientation switch
        {
            DisplayOrientation.Landscape => Orientation.Landscape,
            DisplayOrientation.Portrait => Orientation.Portrait,
            _ => Orientation.Unknown
        };

    public async ValueTask<Stream> TakeScreenshotAsync()
    {
        var result = await screenshot.CaptureAsync();
        return await result.OpenReadAsync();
    }

    public void KeepScreenOn(bool value) => deviceDisplay.KeepScreenOn = value;
}
