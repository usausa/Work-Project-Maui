namespace Template.MobileApp.Controls;

public sealed class MixerSlider : GraphicsView, IDrawable
{
    private const float TickWidth = 1f;

    private const double ThumbTouchMargin = 12;

    private const float ThumbBandRate = 0.25f;

    private static readonly Point ThumbGradientStartPoint = new(0, 0);

    private static readonly Point ThumbGradientEndPoint = new(0, 1);

    //--------------------------------------------------------------------------------
    // Member
    //--------------------------------------------------------------------------------

    private bool isDragging;

    //--------------------------------------------------------------------------------
    // Property
    //--------------------------------------------------------------------------------

    public static readonly BindableProperty ValueProperty = BindableProperty.Create(
        nameof(Value),
        typeof(double),
        typeof(MixerSlider),
        0.0,
        propertyChanged: OnPropertyChanged, defaultBindingMode: BindingMode.TwoWay);

    public static readonly BindableProperty MinimumProperty = BindableProperty.Create(
        nameof(Minimum),
        typeof(double),
        typeof(MixerSlider),
        0.0,
        propertyChanged: OnPropertyChanged);

    public static readonly BindableProperty MaximumProperty = BindableProperty.Create(
        nameof(Maximum),
        typeof(double),
        typeof(MixerSlider),
        100.0,
        propertyChanged: OnPropertyChanged);

    public static readonly BindableProperty TrackColorProperty = BindableProperty.Create(
        nameof(TrackColor),
        typeof(Color),
        typeof(MixerSlider),
        Colors.DimGray,
        propertyChanged: OnPropertyChanged);

    public static readonly BindableProperty ProgressColorProperty = BindableProperty.Create(
        nameof(ProgressColor),
        typeof(Color),
        typeof(MixerSlider),
        Colors.OrangeRed,
        propertyChanged: OnPropertyChanged);

    public static readonly BindableProperty ThumbColor1Property = BindableProperty.Create(
        nameof(ThumbColor1),
        typeof(Color),
        typeof(MixerSlider),
        Colors.LightGray,
        propertyChanged: OnPropertyChanged);

    public static readonly BindableProperty ThumbColor2Property = BindableProperty.Create(
        nameof(ThumbColor2),
        typeof(Color),
        typeof(MixerSlider),
        Colors.DarkGray,
        propertyChanged: OnPropertyChanged);

    public static readonly BindableProperty TrackWidthProperty = BindableProperty.Create(
        nameof(TrackWidth),
        typeof(double),
        typeof(MixerSlider),
        16.0,
        propertyChanged: OnPropertyChanged);

    public static readonly BindableProperty ThumbWidthProperty = BindableProperty.Create(
        nameof(ThumbWidth),
        typeof(double),
        typeof(MixerSlider), 32.0,
        propertyChanged: OnPropertyChanged);

    public static readonly BindableProperty ThumbHeightProperty = BindableProperty.Create(
        nameof(ThumbHeight),
        typeof(double),
        typeof(MixerSlider),
        12.0,
        propertyChanged: OnPropertyChanged);

    public static readonly BindableProperty HasTickMarksProperty = BindableProperty.Create(
        nameof(HasTickMarks),
        typeof(bool),
        typeof(MixerSlider),
        true,
        propertyChanged: OnPropertyChanged);

    public static readonly BindableProperty TickMarkColorProperty = BindableProperty.Create(
        nameof(TickMarkColor),
        typeof(Color),
        typeof(MixerSlider),
        Colors.Gray,
        propertyChanged: OnPropertyChanged);

    public static readonly BindableProperty TickMarkCountProperty = BindableProperty.Create(
        nameof(TickMarkCount),
        typeof(int),
        typeof(MixerSlider),
        11,
        propertyChanged: OnPropertyChanged);

    public static readonly BindableProperty TickMarkLengthProperty = BindableProperty.Create(
        nameof(TickMarkLength),
        typeof(double),
        typeof(MixerSlider),
        24.0,
        propertyChanged: OnPropertyChanged);

    public double Value
    {
        get => (double)GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    public double Minimum
    {
        get => (double)GetValue(MinimumProperty);
        set => SetValue(MinimumProperty, value);
    }

    public double Maximum
    {
        get => (double)GetValue(MaximumProperty);
        set => SetValue(MaximumProperty, value);
    }

    public Color TrackColor
    {
        get => (Color)GetValue(TrackColorProperty);
        set => SetValue(TrackColorProperty, value);
    }

    public Color ProgressColor
    {
        get => (Color)GetValue(ProgressColorProperty);
        set => SetValue(ProgressColorProperty, value);
    }

    public Color ThumbColor1
    {
        get => (Color)GetValue(ThumbColor1Property);
        set => SetValue(ThumbColor1Property, value);
    }

    public Color ThumbColor2
    {
        get => (Color)GetValue(ThumbColor2Property);
        set => SetValue(ThumbColor2Property, value);
    }

    public double TrackWidth
    {
        get => (double)GetValue(TrackWidthProperty);
        set => SetValue(TrackWidthProperty, value);
    }

    public double ThumbWidth
    {
        get => (double)GetValue(ThumbWidthProperty);
        set => SetValue(ThumbWidthProperty, value);
    }

    public double ThumbHeight
    {
        get => (double)GetValue(ThumbHeightProperty);
        set => SetValue(ThumbHeightProperty, value);
    }

    public bool HasTickMarks
    {
        get => (bool)GetValue(HasTickMarksProperty);
        set => SetValue(HasTickMarksProperty, value);
    }

    public Color TickMarkColor
    {
        get => (Color)GetValue(TickMarkColorProperty);
        set => SetValue(TickMarkColorProperty, value);
    }

    public int TickMarkCount
    {
        get => (int)GetValue(TickMarkCountProperty);
        set => SetValue(TickMarkCountProperty, value);
    }

    public double TickMarkLength
    {
        get => (double)GetValue(TickMarkLengthProperty);
        set => SetValue(TickMarkLengthProperty, value);
    }

    //--------------------------------------------------------------------------------
    // Constructor
    //--------------------------------------------------------------------------------

    public MixerSlider()
    {
        Drawable = this;

        StartInteraction += (_, e) => OnStartInteraction(e.Touches[0]);
        DragInteraction += (_, e) => OnDragInteraction(e.Touches[0]);
        EndInteraction += (_, _) => OnEndInteraction();
    }

    //--------------------------------------------------------------------------------
    // Render
    //--------------------------------------------------------------------------------

    private static void OnPropertyChanged(BindableObject bindable, object oldValue, object newValue)
    {
        ((MixerSlider)bindable).Invalidate();
    }

    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        var thumbX = dirtyRect.Width / 2;

        var valuePercentage = (Value - Minimum) / (Maximum - Minimum);

        var trackTop = (float)(ThumbHeight / 2);
        var trackBottom = dirtyRect.Height - (float)(ThumbHeight / 2);
        var trackHeight = trackBottom - trackTop;

        var thumbY = (float)(trackBottom - (valuePercentage * trackHeight));

        if (HasTickMarks && (TickMarkCount > 1))
        {
            canvas.StrokeColor = TickMarkColor;
            canvas.StrokeSize = TickWidth;

            var tickStart = thumbX - (float)TickMarkLength;
            var tickEnd = thumbX + (float)TickMarkLength;
            for (var i = 0; i < TickMarkCount; i++)
            {
                var tickPercentage = (float)i / (TickMarkCount - 1);
                var tickY = trackBottom - (tickPercentage * trackHeight);
                canvas.DrawLine(tickStart, tickY, tickEnd, tickY);
            }
        }

        canvas.StrokeSize = (float)TrackWidth;
        canvas.StrokeColor = TrackColor;
        canvas.DrawLine(thumbX, trackTop - TickWidth, thumbX, trackBottom + TickWidth);

        canvas.StrokeColor = ProgressColor;
        canvas.DrawLine(thumbX, thumbY - TickWidth, thumbX, trackBottom + TickWidth);

        var thumbHalfWidth = (float)(ThumbWidth / 2);
        var thumbHalfHeight = (float)(ThumbHeight / 2);

        var thumbRect = new RectF(thumbX - thumbHalfWidth, thumbY - thumbHalfHeight, (float)ThumbWidth, (float)ThumbHeight);

        canvas.FillColor = ThumbColor1;
        canvas.FillRectangle(new RectF(thumbRect.X, thumbRect.Y, thumbRect.Width, thumbRect.Height * ThumbBandRate));

        canvas.FillColor = ThumbColor2;
        canvas.FillRectangle(new RectF(thumbRect.X, thumbRect.Y + (thumbRect.Height * (1 - ThumbBandRate)), thumbRect.Width, thumbRect.Height * ThumbBandRate));

        var gradientBrush = new LinearGradientBrush
        {
            StartPoint = ThumbGradientStartPoint,
            EndPoint = ThumbGradientEndPoint
        };
        gradientBrush.GradientStops.Add(new GradientStop(ThumbColor2, 0));
        gradientBrush.GradientStops.Add(new GradientStop(ThumbColor1, 1));

        var thumbMiddleRect = new RectF(thumbRect.X, thumbRect.Y + (thumbRect.Height * ThumbBandRate), thumbRect.Width, thumbRect.Height * (1 - (ThumbBandRate * 2)));
        canvas.SetFillPaint(gradientBrush, thumbMiddleRect);
        canvas.FillRectangle(thumbMiddleRect);
    }

    //--------------------------------------------------------------------------------
    // Touch
    //--------------------------------------------------------------------------------

    private void OnStartInteraction(PointF point)
    {
        if (IsPointInThumb(point))
        {
            isDragging = true;
        }
        UpdateValue(point);
    }

    private void OnDragInteraction(PointF point)
    {
        if (isDragging)
        {
            UpdateValue(point);
        }
    }

    private void OnEndInteraction()
    {
        isDragging = false;
    }

    private void UpdateValue(PointF point)
    {
        var trackHeight = Height - ThumbHeight;
        var thumbPosition = point.Y - (ThumbHeight / 2);

        var percentage = 1 - Math.Clamp(thumbPosition / trackHeight, 0, 1);
        Value = Minimum + (percentage * (Maximum - Minimum));
    }

    private bool IsPointInThumb(PointF point)
    {
        var thumbX = Width / 2;

        var valuePercentage = (Value - Minimum) / (Maximum - Minimum);
        var trackTop = ThumbHeight / 2;
        var trackBottom = Height - (ThumbHeight / 2);
        var trackHeight = trackBottom - trackTop;
        var thumbY = (float)(trackBottom - (valuePercentage * trackHeight));

        var thumbRect = new RectF(
            (float)(thumbX - (ThumbWidth / 2) - ThumbTouchMargin),
            (float)(thumbY - (ThumbHeight / 2) - ThumbTouchMargin),
            (float)(ThumbWidth + (ThumbTouchMargin * 2)),
            (float)(ThumbHeight + (ThumbTouchMargin * 2)));

        return thumbRect.Contains(point);
    }
}
