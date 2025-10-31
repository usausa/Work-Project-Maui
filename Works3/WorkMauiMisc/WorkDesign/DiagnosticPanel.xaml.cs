namespace WorkDesign;

public partial class DiagnosticPanel
{
    private bool isMonitoring;

    private float fps;
    private float frameTime;

    private float cpuUsage;

    private int threads;

    private float memoryUsed;

    private int gc0Delta;
    private int gc1Delta;
    private int gc2Delta;

    private float allocPerSec;

    private int battery;

    public static readonly BindableProperty SafeColorProperty = BindableProperty.Create(
        nameof(SafeColor),
        typeof(Color),
        typeof(DiagnosticPanel),
        Colors.Green,
        propertyChanged: OnPropertyChanged);

    public Color SafeColor
    {
        get => (Color)GetValue(SafeColorProperty);
        set => SetValue(SafeColorProperty, value);
    }

    public static readonly BindableProperty WarningColorProperty = BindableProperty.Create(
        nameof(WarningColor),
        typeof(Color),
        typeof(DiagnosticPanel),
        Colors.Orange,
        propertyChanged: OnPropertyChanged);

    public Color WarningColor
    {
        get => (Color)GetValue(WarningColorProperty);
        set => SetValue(WarningColorProperty, value);
    }

    public static readonly BindableProperty CriticalColorProperty = BindableProperty.Create(
        nameof(CriticalColor),
        typeof(Color),
        typeof(DiagnosticPanel),
        Colors.Red,
        propertyChanged: OnPropertyChanged);

    public Color CriticalColor
    {
        get => (Color)GetValue(CriticalColorProperty);
        set => SetValue(CriticalColorProperty, value);
    }

    public static readonly BindableProperty DisableColorProperty = BindableProperty.Create(
        nameof(DisableColor),
        typeof(Color),
        typeof(DiagnosticPanel),
        Colors.Gray,
        propertyChanged: OnPropertyChanged);

    public Color DisableColor
    {
        get => (Color)GetValue(DisableColorProperty);
        set => SetValue(DisableColorProperty, value);
    }

    public DiagnosticPanel()
    {
        InitializeComponent();

        HandlerChanged += OnHandlerChanged;
    }

    private static void OnPropertyChanged(BindableObject bindable, object oldValue, object newValue)
    {
        ((DiagnosticPanel)bindable).UpdateValues();
    }

    private void OnHandlerChanged(object? sender, EventArgs e)
    {
        if (Handler is null)
        {
            StopMonitor();
        }
    }

    protected override void OnPropertyChanged(string? propertyName = null)
    {
        base.OnPropertyChanged(propertyName);

        if (propertyName == nameof(IsVisible))
        {
            StartMonitor();
        }
    }

    private void StartMonitor()
    {
        if (isMonitoring)
        {
            return;
        }

        // TODO
        fps = 60.0f;
        frameTime = 16.6f;

        cpuUsage = 100.0f;
        threads = 256;

        memoryUsed = 256.0f;
        gc0Delta = 0;
        gc1Delta = 0;
        gc2Delta = 0;
        allocPerSec = 10.0f;

        battery = 1000;

        UpdateValues();

        isMonitoring = true;
    }

    private void StopMonitor()
    {
        if (!isMonitoring)
        {
            return;
        }

        // TODO

        isMonitoring = false;
    }

    private void UpdateValues()
    {
        var safeColor = SafeColor;
        var warningColor = WarningColor;
        var criticalColor = CriticalColor;
        // TODO
        var disableColor = DisableColor;

        FpsLabel.Text = $"{fps:F1}";
        FpsLabel.TextColor = fps switch
        {
            >= 50 => safeColor,
            >= 30 => warningColor,
            _ => criticalColor
        };

        FrameTimeLabel.Text = $"{frameTime:F1}ms";
        FrameTimeLabel.TextColor = frameTime switch
        {
            <= 16.6f => safeColor,
            <= 33.3f => warningColor,
            _ => criticalColor
        };

        CpuLabel.Text = $"{cpuUsage:F1}%";
        CpuLabel.TextColor = cpuUsage switch
        {
            <= 30.0f => safeColor,
            <= 60.0f => warningColor,
            _ => criticalColor
        };

        ThreadsLabel.Text = $"{threads}";
        ThreadsLabel.TextColor = threads switch
        {
            <= 64 => safeColor,
            <= 128 => warningColor,
            _ => criticalColor
        };

        MemoryLabel.Text = $"{memoryUsed:F1}MB";
        MemoryLabel.TextColor = memoryUsed switch
        {
            <= 256.0f => safeColor,
            <= 512.0f => warningColor,
            _ => criticalColor
        };

        Gc0Label.Text = $"{gc0Delta}";
        Gc1Label.Text = $"{gc1Delta}";
        Gc2Label.Text = $"{gc2Delta}";
        var gcColor = (gc0Delta + gc1Delta + gc2Delta) switch
        {
            0 => safeColor,
            _ => criticalColor
        };
        Gc0Label.TextColor = gcColor;
        Gc1Label.TextColor = gcColor;
        Gc2Label.TextColor = gcColor;

        AllocLabel.Text = $"{allocPerSec:F1}MB";
        AllocLabel.TextColor = allocPerSec switch
        {
            <= 4.0f => safeColor,
            <= 8.0f => warningColor,
            _ => criticalColor
        };

        BatteryLabel.Text = $"{battery}";
        BatteryLabel.TextColor = battery switch
        {
            >= 500 => safeColor,
            >= 100 => warningColor,
            _ => criticalColor
        };
    }
}

