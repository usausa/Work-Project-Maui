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

    // Information

    string? GetVersion();
}
