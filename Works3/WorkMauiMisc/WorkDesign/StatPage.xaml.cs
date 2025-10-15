namespace WorkDesign;

using SkiaSharp.Views.Maui;

using Smart.Maui.ViewModels;

public partial class StatPage : ContentPage
{
	public StatPage()
	{
		InitializeComponent();
	}
}

public sealed class StatPageViewModel : ExtendViewModelBase
{
    private readonly IDispatcherTimer timer;

    private int counter = 0;

    public StatDataSet CpuLoadSet { get; }

    public StatPageViewModel()
    {
        CpuLoadSet = new StatDataSet(100);

        timer = Application.Current?.Dispatcher.CreateTimer()!;
        timer.Interval = TimeSpan.FromSeconds(1);
        timer.Tick += OnTimerTick;
    }

    private void OnTimerTick(object? sender, EventArgs e)
    {
        CpuLoadSet.Add(counter);

        counter++;
        if (counter > 100)
        {
            counter = 0;
        }
    }
}

public sealed class StatControl : GraphicsView, IDrawable
{
    // ------------------------------------------------------------
    // Property
    // ------------------------------------------------------------

    public static readonly BindableProperty GraphColorProperty = BindableProperty.Create(
        nameof(GraphColor),
        typeof(Color),
        typeof(StatControl),
        Colors.Blue,
        propertyChanged: OnPropertyChanged);

    public Color GraphColor
    {
        get => (Color)GetValue(GraphColorProperty);
        set => SetValue(GraphColorProperty, value);
    }

    public static readonly BindableProperty LabelProperty = BindableProperty.Create(
        nameof(Label),
        typeof(string),
        typeof(StatControl),
        "Usage",
        propertyChanged: OnPropertyChanged);

    public string Label
    {
        get => (string)GetValue(LabelProperty);
        set => SetValue(LabelProperty, value);
    }

    public static readonly BindableProperty UnitProperty = BindableProperty.Create(
        nameof(Unit),
        typeof(string),
        typeof(StatControl),
        "%",
        propertyChanged: OnPropertyChanged);

    public string Unit
    {
        get => (string)GetValue(UnitProperty);
        set => SetValue(UnitProperty, value);
    }

    public static readonly BindableProperty MaxValueProperty = BindableProperty.Create(
        nameof(MaxValue),
        typeof(float),
        typeof(StatControl),
        100.0f,
        propertyChanged: OnPropertyChanged);

    public float MaxValue
    {
        get => (float)GetValue(MaxValueProperty);
        set => SetValue(MaxValueProperty, value);
    }

    public static readonly BindableProperty DataSetProperty = BindableProperty.Create(
        nameof(DataSet),
        typeof(StatDataSet),
        typeof(StatControl),
        propertyChanged: OnDataSetChanged);


    public StatDataSet DataSet
    {
        get => (StatDataSet)GetValue(DataSetProperty);
        set => SetValue(DataSetProperty, value);
    }

    // ------------------------------------------------------------
    // Handler
    // ------------------------------------------------------------

    private static void OnPropertyChanged(BindableObject bindable, object oldValue, object newValue)
    {
        ((StatControl)bindable).Invalidate();
    }

    private static void OnDataSetChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (StatControl)bindable;

        if (oldValue is StatDataSet oldDataSet)
        {
            oldDataSet.Updated -= control.OnDataSetUpdated;
        }

        if (newValue is StatDataSet newDataSet)
        {
            newDataSet.Updated += control.OnDataSetUpdated;
        }
    }

    private void OnDataSetUpdated(object? sender, EventArgs e)
    {
        Invalidate();
    }

    // ------------------------------------------------------------
    // Draw
    // ------------------------------------------------------------

    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        var width = dirtyRect.Width;
        var height = dirtyRect.Height;

        var values = DataSet;
        var pointWidth = (float)width / (values.Capacity - 1);
        var maxValueForScale = MaxValue > 0 ? MaxValue : 100f;
        var color = GraphColor;

        canvas.SaveState();
        canvas.Antialias = true;

        // TODO ctrl
        // TODO

        canvas.RestoreState();
    }
}

public sealed class StatDataSet
{
    public event EventHandler<EventArgs>? Updated;

    private readonly int capacity;

    private readonly float[] buffer;

    private int head;

    public int Capacity => capacity;

    public StatDataSet(int capacity)
    {
        this.capacity = capacity;
        buffer = new float[capacity];
    }

    public void Add(float value)
    {
        var index = head;
        head = (head + 1) % capacity;
        buffer[index] = value;

        Updated?.Invoke(this, EventArgs.Empty);
    }

    public float GetValue(int index)
    {
        var actualIndex = (head + index) % capacity;
        return buffer[actualIndex];
    }

    public float GetLastValue()
    {
        var actualIndex = head == 0 ? capacity - 1 : head - 1;
        return buffer[actualIndex];
    }
}
