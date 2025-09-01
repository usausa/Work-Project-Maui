namespace WorkDesign;

public partial class JoyStick
{
    // Size

    public static readonly BindableProperty ThumbSizeProperty = BindableProperty.Create(
        nameof(ThumbSize),
        typeof(double),
        typeof(JoyStick),
        80.0);

    public double ThumbSize
    {
        get => (double)GetValue(ThumbSizeProperty);
        set => SetValue(ThumbSizeProperty, value);
    }

    public static readonly BindableProperty RangeOfMotionProperty = BindableProperty.Create(
        nameof(RangeOfMotion),
        typeof(double),
        typeof(JoyStick),
        56.0);

    public double RangeOfMotion
    {
        get => (double)GetValue(RangeOfMotionProperty);
        set => SetValue(RangeOfMotionProperty, value);
    }

    public static readonly BindableProperty ThumbBorderWidthProperty = BindableProperty.Create(
        nameof(ThumbBorderWidth),
        typeof(double),
        typeof(JoyStick),
        8.0);

    public double ThumbBorderWidth
    {
        get => (double)GetValue(ThumbBorderWidthProperty);
        set => SetValue(ThumbBorderWidthProperty, value);
    }

    // Color

    public static readonly BindableProperty BaseBackgroundProperty = BindableProperty.Create(
        nameof(BaseBackground),
        typeof(Brush),
        typeof(JoyStick),
        Brush.LightGray,
        propertyChanged: OnPropertyChanged);

    public Brush BaseBackground
    {
        get => (Brush)GetValue(BaseBackgroundProperty);
        set => SetValue(BaseBackgroundProperty, value);
    }

    public static readonly BindableProperty ThumbBackgroundProperty = BindableProperty.Create(
        nameof(ThumbBackground),
        typeof(Brush),
        typeof(JoyStick),
        Brush.Red,
        propertyChanged: OnPropertyChanged);

    public Brush ThumbBackground
    {
        get => (Brush)GetValue(ThumbBackgroundProperty);
        set => SetValue(ThumbBackgroundProperty, value);
    }

    public static readonly BindableProperty ThumbBorderBackgroundProperty = BindableProperty.Create(
        nameof(ThumbBorderBackground),
        typeof(Brush),
        typeof(JoyStick),
        Brush.DarkRed,
        propertyChanged: OnPropertyChanged);

    public Brush ThumbBorderBackground
    {
        get => (Brush)GetValue(ThumbBorderBackgroundProperty);
        set => SetValue(ThumbBorderBackgroundProperty, value);
    }

    public static readonly BindableProperty ArrowColorProperty = BindableProperty.Create(
        nameof(ArrowColor),
        typeof(Color),
        typeof(JoyStick),
        Colors.White);

    public Color ArrowColor
    {
        get => (Color)GetValue(ArrowColorProperty);
        set => SetValue(ArrowColorProperty, value);
    }

    // Arrow
    public static readonly BindableProperty ArrowMarginRatioProperty =
        BindableProperty.Create(nameof(ArrowMarginRatio), typeof(double), typeof(JoyStick),
            0.1d, propertyChanged: OnPropertyChanged);

    public double ArrowMarginRatio
    {
        get => (double)GetValue(ArrowMarginRatioProperty);
        set => SetValue(ArrowMarginRatioProperty, value);
    }

    public static readonly BindableProperty ArrowHeightRatioProperty =
        BindableProperty.Create(nameof(ArrowHeightRatio), typeof(double), typeof(JoyStick),
            0.2d, propertyChanged: OnPropertyChanged);

    public double ArrowHeightRatio
    {
        get => (double)GetValue(ArrowHeightRatioProperty);
        set => SetValue(ArrowHeightRatioProperty, value);
    }

    public static readonly BindableProperty ArrowHalfWidthRatioProperty =
        BindableProperty.Create(nameof(ArrowHalfWidthRatio), typeof(double), typeof(JoyStick),
            0.2d, propertyChanged: OnPropertyChanged);

    public double ArrowHalfWidthRatio
    {
        get => (double)GetValue(ArrowHalfWidthRatioProperty);
        set => SetValue(ArrowHalfWidthRatioProperty, value);
    }

    // Value

    public static readonly BindableProperty XValueProperty = BindableProperty.Create(
        nameof(XValue),
        typeof(double),
        typeof(JoyStick),
        0.0,
        defaultBindingMode: BindingMode.OneWayToSource);

    public double XValue
    {
        get => (double)GetValue(XValueProperty);
        private set => SetValue(XValueProperty, value);
    }

    public static readonly BindableProperty YValueProperty = BindableProperty.Create(
        nameof(YValue),
        typeof(double),
        typeof(JoyStick),
        0.0,
        defaultBindingMode: BindingMode.OneWayToSource);

    public double YValue
    {
        get => (double)GetValue(YValueProperty);
        private set => SetValue(YValueProperty, value);
    }


    public JoyStick()
	{
		InitializeComponent();

        SizeChanged += (_, _) => Update();

        var panGesture = new PanGestureRecognizer();
        panGesture.PanUpdated += OnPanUpdated;
        ThumbGrid.GestureRecognizers.Add(panGesture);
    }

    private static void OnPropertyChanged(BindableObject bindable, object oldValue, object newValue)
    {
        ((JoyStick)bindable).Update();
    }

    private void Update()
    {
        if ((Width <= 0) || (Height <= 0))
        {
            return;
        }

        var size = Math.Min(Width, Height);
        var r = size / 2.0;
        var cx = Width / 2.0;
        var cy = Height / 2.0;

        // Base
        BaseCircle.WidthRequest = size;
        BaseCircle.HeightRequest = size;

        // Arrow
        var margin = r * ArrowMarginRatio;
        var h = r * ArrowHeightRatio;
        var w = r * ArrowHalfWidthRatio;

        // Up
        var apexUp = new Point(cx, cy - r + margin);
        UpArrow.Points = [apexUp, new Point(cx - w, apexUp.Y + h), new Point(cx + w, apexUp.Y + h)];
        // Down
        var apexDown = new Point(cx, cy + r - margin);
        DownArrow.Points = [apexDown, new Point(cx + w, apexDown.Y - h), new Point(cx - w, apexDown.Y - h)];
        // Left
        var apexLeft = new Point(cx - r + margin, cy);
        LeftArrow.Points = [apexLeft, new Point(apexLeft.X + h, cy - w), new Point(apexLeft.X + h, cy + w)];
        // Right
        var apexRight = new Point(cx + r - margin, cy);
        RightArrow.Points = [apexRight, new Point(apexRight.X - h, cy + w), new Point(apexRight.X - h, cy - w)];
    }

    private void OnPanUpdated(object? sender, PanUpdatedEventArgs e)
    {
        var radius = RangeOfMotion;
        switch (e.StatusType)
        {
            case GestureStatus.Started:
            case GestureStatus.Running:
                var (x, y) = ClampToCircle(radius, e.TotalX, e.TotalY);
                ThumbBorder.TranslationX = x;
                ThumbBorder.TranslationY = y;
                Thumb.TranslationX = x;
                Thumb.TranslationY = y;

                XValue = x / radius;
                YValue = y / radius;
                break;

            case GestureStatus.Completed:
            case GestureStatus.Canceled:
                ThumbBorder.TranslationX = 0;
                ThumbBorder.TranslationY = 0;
                Thumb.TranslationX = 0;
                Thumb.TranslationY = 0;

                XValue = 0;
                YValue = 0;
                break;
        }
    }

    public (double X, double Y) ClampToCircle(double radius, double x, double y)
    {
        var distance = Math.Sqrt(x * x + y * y);
        return distance > radius ? (x * (radius / distance), y * (radius / distance)) : (x, y);
    }
}