namespace WorkDesign;

using System.Globalization;
using Smart.Maui.ViewModels;
using Smart.Mvvm;

public partial class DiagnosticPanel : ContentView
{
	public DiagnosticPanel()
	{
		InitializeComponent();
	}
}

public sealed partial class DiagnosticPanelViewModel : ExtendViewModelBase
{
    // FPS

    [ObservableProperty]
    public partial float FpsValue { get; set; }

    [ObservableProperty]
    public partial AlertLevel FpsAlert { get; set; }

    [ObservableProperty]
    public partial float FrameValue { get; set; }

    [ObservableProperty]
    public partial AlertLevel FrameAlert { get; set; }

    // System

    [ObservableProperty]
    public partial float CpuValue { get; set; }

    [ObservableProperty]
    public partial AlertLevel CpuAlert { get; set; }

    [ObservableProperty]
    public partial int ThreadsValue { get; set; }

    [ObservableProperty]
    public partial AlertLevel ThreadsAlert { get; set; }

    // Memory

    [ObservableProperty]
    public partial float MemoryValue { get; set; }

    [ObservableProperty]
    public partial AlertLevel MemoryAlert { get; set; }

    [ObservableProperty]
    public partial int Gc0Value { get; set; }
    [ObservableProperty]
    public partial int Gc1Value { get; set; }
    [ObservableProperty]
    public partial int Gc2Value { get; set; }

    [ObservableProperty]
    public partial AlertLevel GcAlert { get; set; }

    [ObservableProperty]
    public partial float AllocValue { get; set; }

    [ObservableProperty]
    public partial AlertLevel AllocAlert { get; set; }

    // Battery


    [ObservableProperty]
    public partial int BatteryValue { get; set; }

    [ObservableProperty]
    public partial AlertLevel BatteryAlert { get; set; }

    public DiagnosticPanelViewModel()
    {
        FpsValue = 60.0f;
        FrameValue = 16.6f;

        CpuValue = 100.0f;
        ThreadsValue = 256;

        MemoryValue = 256.0f;
        Gc0Value = 0;
        Gc1Value = 0;
        Gc2Value = 0;
        AllocValue = 10.0f;

        BatteryValue = 1000;

        UpdateAlertFps();
        UpdateAlertFrame();
        UpdateAlertCpu();
        UpdateAlertThreads();
        UpdateAlertMemory();
        UpdateAlertGc();
        UpdateAlertAlloc();
        UpdateAlertBattery();
    }

    // Update

    private void UpdateAlertFps()
    {
        FpsAlert = FpsValue switch
        {
            >= 50 => AlertLevel.Safe,
            >= 30 => AlertLevel.Warning,
            _ => AlertLevel.Critical
        };
    }

    private void UpdateAlertFrame()
    {
        FrameAlert = FrameValue switch
        {
            <= 16.6f => AlertLevel.Safe,
            <= 33.3f => AlertLevel.Warning,
            _ => AlertLevel.Critical
        };
    }

    private void UpdateAlertCpu()
    {
        CpuAlert = CpuValue switch
        {
            <= 30.0f => AlertLevel.Safe,
            <= 60.0f => AlertLevel.Warning,
            _ => AlertLevel.Critical
        };
    }

    private void UpdateAlertThreads()
    {
        ThreadsAlert = ThreadsValue switch
        {
            <= 64 => AlertLevel.Safe,
            <= 128 => AlertLevel.Warning,
            _ => AlertLevel.Critical
        };
    }

    private void UpdateAlertMemory()
    {
        MemoryAlert = MemoryValue switch
        {
            <= 256.0f => AlertLevel.Safe,
            <= 512.0f => AlertLevel.Warning,
            _ => AlertLevel.Critical
        };
    }

    private void UpdateAlertGc()
    {
        GcAlert = (Gc0Value + Gc1Value + Gc2Value) switch
        {
            0 => AlertLevel.Safe,
            _ => AlertLevel.Critical
        };
    }

    private void UpdateAlertAlloc()
    {
        AllocAlert = AllocValue switch
        {
            <= 4.0f => AlertLevel.Safe,
            <= 8.0f => AlertLevel.Warning,
            _ => AlertLevel.Critical
        };
    }

    private void UpdateAlertBattery()
    {
        BatteryAlert = BatteryValue switch
        {
            >= 500 => AlertLevel.Safe,
            >= 100 => AlertLevel.Warning,
            _ => AlertLevel.Critical
        };
    }
}

public enum AlertLevel
{
    Safe,
    Warning,
    Critical
}

public sealed class AlertLevelToColorConverter : IValueConverter
{
    public Color SafeColor { get; set; } = Colors.Green;

    public Color WarningColor { get; set; } = Colors.Orange;

    public Color CriticalColor { get; set; } = Colors.Red;

    public Color UnknownColor { get; set; } = Colors.Gray;

    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is AlertLevel level)
        {
            return level switch
            {
                AlertLevel.Safe => SafeColor,
                AlertLevel.Warning => WarningColor,
                AlertLevel.Critical => CriticalColor,
                _ => UnknownColor
            };
        }

        return UnknownColor;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        throw new NotSupportedException();
}
