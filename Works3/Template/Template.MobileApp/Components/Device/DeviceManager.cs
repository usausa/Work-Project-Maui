namespace Template.MobileApp.Components.Device;

public interface IDeviceManager
{
    // Status

    IObservable<NetworkState> NetworkState { get; }

    NetworkState GetNetworkState();

    // Display

    void SetOrientation(Orientation orientation);

    // Feed

    void Vibrate(double duration);

    void VibrateCancel();

    void FeedbackClick();

    void FeedbackLongPress();

    // Light

    void LightOn();

    void LightOff();

    // Screen

    ValueTask<Stream> TakeScreenshotAsync();

    // Information

    string? GetVersion();
}

public sealed partial class DeviceManager : IDeviceManager, IDisposable
{
    private readonly IVibration vibration;

    private readonly IHapticFeedback feedback;

    private readonly IFlashlight flashlight;

    private readonly IScreenshot screenshot;

    private readonly BehaviorSubject<NetworkState> networkState;

    public IObservable<NetworkState> NetworkState => networkState;

    public DeviceManager(
        IVibration vibration,
        IHapticFeedback feedback,
        IFlashlight flashlight,
        IScreenshot screenshot)
    {
        this.vibration = vibration;
        this.feedback = feedback;
        this.flashlight = flashlight;
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
    // Feed
    // ------------------------------------------------------------

    public void Vibrate(double duration) => vibration.Vibrate(duration);

    public void VibrateCancel() => vibration.Cancel();

    public void FeedbackClick() => feedback.Perform(HapticFeedbackType.Click);

    public void FeedbackLongPress() => feedback.Perform(HapticFeedbackType.LongPress);

    // ------------------------------------------------------------
    // Light
    // ------------------------------------------------------------

    public void LightOn() => flashlight.TurnOnAsync();

    public void LightOff() => flashlight.TurnOffAsync();

    // ------------------------------------------------------------
    // Screen
    // ------------------------------------------------------------

    public async ValueTask<Stream> TakeScreenshotAsync()
    {
        var result = await screenshot.CaptureAsync();
        return await result.OpenReadAsync();
    }
}
