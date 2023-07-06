namespace Template.MobileApp.State;

public sealed class DeviceState : NotificationObject, IDisposable
{
    private readonly ILogger<DeviceState> log;

    private readonly List<IDisposable> disposables = new();

    // Battery

    private double batteryChargeLevel;

    public double BatteryChargeLevel
    {
        get => batteryChargeLevel;
        set => SetProperty(ref batteryChargeLevel, value);
    }

    private BatteryState batteryState;

    public BatteryState BatteryState
    {
        get => batteryState;
        set => SetProperty(ref batteryState, value);
    }

    private BatteryPowerSource batteryPowerSource;

    public BatteryPowerSource BatteryPowerSource
    {
        get => batteryPowerSource;
        set => SetProperty(ref batteryPowerSource, value);
    }

    public DeviceState(
        ILogger<DeviceState> log,
        IBattery battery)
    {
        this.log = log;

        // Battery
        UpdateBattery(battery.ChargeLevel, battery.State, battery.PowerSource);
        disposables.Add(Observable
            .FromEvent<EventHandler<BatteryInfoChangedEventArgs>, BatteryInfoChangedEventArgs>(h => (_, e) => h(e), h => battery.BatteryInfoChanged += h, h => battery.BatteryInfoChanged -= h)
            .ObserveOn(SynchronizationContext.Current!)
            .Subscribe(x => UpdateBattery(x.ChargeLevel, x.State, x.PowerSource)));
    }

    private void UpdateBattery(double chargeLevel, BatteryState state, BatteryPowerSource powerSource)
    {
#pragma warning disable CA2254
#pragma warning disable CA1848
        log.LogDebug($"Battery info changed. level=[{chargeLevel}], state=[{state}], source=[{powerSource}]");
#pragma warning restore CA1848
#pragma warning restore CA2254

        BatteryChargeLevel = chargeLevel;
        BatteryState = state;
        BatteryPowerSource = powerSource;
    }

    public void Dispose()
    {
        foreach (var disposable in disposables)
        {
            disposable.Dispose();
        }

        disposables.Clear();
    }
}
