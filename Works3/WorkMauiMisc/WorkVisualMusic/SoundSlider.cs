namespace WorkVisualMusic;

using System;
using System.Windows.Input;

public class SoundSliderValueChangedEventArgs : EventArgs
{
    public double Value { get; }

    public SoundSliderValueChangedEventArgs(double value)
    {
        Value = value;
    }
}

public sealed class SoundSlider : GraphicsView, IDrawable
{
    private const float TickWidth = 1f;

    // ------------------------------------------------------------
    // Property
    // ------------------------------------------------------------

    // Value

    public static readonly BindableProperty ValueProperty = BindableProperty.Create(
        nameof(Value),
        typeof(double),
        typeof(SoundSlider),
        0.0,
        propertyChanged: OnValueChanged, defaultBindingMode: BindingMode.TwoWay);

    public double Value
    {
        get => (double)GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    public static readonly BindableProperty MinimumProperty = BindableProperty.Create(
        nameof(Minimum),
        typeof(double),
        typeof(SoundSlider),
        0.0,
        propertyChanged: OnPropertyChanged);

    public double Minimum
    {
        get => (double)GetValue(MinimumProperty);
        set => SetValue(MinimumProperty, value);
    }

    public static readonly BindableProperty MaximumProperty = BindableProperty.Create(
        nameof(Maximum),
        typeof(double),
        typeof(SoundSlider),
        100.0,
        propertyChanged: OnPropertyChanged);

    public double Maximum
    {
        get => (double)GetValue(MaximumProperty);
        set => SetValue(MaximumProperty, value);
    }

    // Color

    public static readonly BindableProperty TrackColorProperty = BindableProperty.Create(
        nameof(TrackColor),
        typeof(Color),
        typeof(SoundSlider),
        Colors.DimGray,
        propertyChanged: OnPropertyChanged);

    public Color TrackColor
    {
        get => (Color)GetValue(TrackColorProperty);
        set => SetValue(TrackColorProperty, value);
    }

    public static readonly BindableProperty ProgressColorProperty = BindableProperty.Create(
        nameof(ProgressColor),
        typeof(Color),
        typeof(SoundSlider),
        Colors.OrangeRed,
        propertyChanged: OnPropertyChanged);

    public Color ProgressColor
    {
        get => (Color)GetValue(ProgressColorProperty);
        set => SetValue(ProgressColorProperty, value);
    }

    public static readonly BindableProperty ThumbColor1Property = BindableProperty.Create(
        nameof(ThumbColor1),
        typeof(Color),
        typeof(SoundSlider),
        Colors.LightGray,
        propertyChanged: OnPropertyChanged);

    public Color ThumbColor1
    {
        get => (Color)GetValue(ThumbColor1Property);
        set => SetValue(ThumbColor1Property, value);
    }

    public static readonly BindableProperty ThumbColor2Property = BindableProperty.Create(
        nameof(ThumbColor2),
        typeof(Color),
        typeof(SoundSlider),
        Colors.DarkGray,
        propertyChanged: OnPropertyChanged);

    public Color ThumbColor2
    {
        get => (Color)GetValue(ThumbColor2Property);
        set => SetValue(ThumbColor2Property, value);
    }

    // Size

    public static readonly BindableProperty TrackWidthProperty = BindableProperty.Create(
        nameof(TrackWidth),
        typeof(double),
        typeof(SoundSlider),
        16.0,
        propertyChanged: OnPropertyChanged);

    public double TrackWidth
    {
        get => (double)GetValue(TrackWidthProperty);
        set => SetValue(TrackWidthProperty, value);
    }

    public static readonly BindableProperty ThumbWidthProperty = BindableProperty.Create(
        nameof(ThumbWidth),
        typeof(double),
        typeof(SoundSlider), 32.0,
        propertyChanged: OnPropertyChanged);

    public double ThumbWidth
    {
        get => (double)GetValue(ThumbWidthProperty);
        set => SetValue(ThumbWidthProperty, value);
    }

    public static readonly BindableProperty ThumbHeightProperty = BindableProperty.Create(
        nameof(ThumbHeight),
        typeof(double),
        typeof(SoundSlider),
        12.0,
        propertyChanged: OnPropertyChanged);

    public double ThumbHeight
    {
        get => (double)GetValue(ThumbHeightProperty);
        set => SetValue(ThumbHeightProperty, value);
    }

    // Tick
    public static readonly BindableProperty HasTickMarksProperty =
        BindableProperty.Create(nameof(HasTickMarks), typeof(bool), typeof(SoundSlider), true,
            propertyChanged: OnPropertyChanged);

    public static readonly BindableProperty TickMarkColorProperty =
        BindableProperty.Create(nameof(TickMarkColor), typeof(Color), typeof(SoundSlider), Colors.Gray,
            propertyChanged: OnPropertyChanged);

    public static readonly BindableProperty TickMarkCountProperty =
        BindableProperty.Create(nameof(TickMarkCount), typeof(int), typeof(SoundSlider), 11,
            propertyChanged: OnPropertyChanged);

    public static readonly BindableProperty TickMarkLengthProperty =
        BindableProperty.Create(nameof(TickMarkLength), typeof(float), typeof(SoundSlider), 24.0f,
            propertyChanged: OnPropertyChanged);

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
    public float TickMarkLength
    {
        get => (float)GetValue(TickMarkLengthProperty);
        set => SetValue(TickMarkLengthProperty, value);
    }

    // Event

    public static readonly BindableProperty ValueChangedCommandProperty = BindableProperty.Create(
        nameof(ValueChangedCommand),
        typeof(ICommand),
        typeof(SoundSlider));

    public ICommand? ValueChangedCommand
    {
        get => (ICommand)GetValue(ValueChangedCommandProperty);
        set => SetValue(ValueChangedCommandProperty, value);
    }

    // ------------------------------------------------------------
    // Field
    // ------------------------------------------------------------

    public event EventHandler<SoundSliderValueChangedEventArgs>? ValueChanged;

    private bool isDragging;

    // ------------------------------------------------------------
    // Constructor
    // ------------------------------------------------------------

    public SoundSlider()
    {
        Drawable = this;

        StartInteraction += (_, e) => OnStartInteraction(e.Touches[0]);
        DragInteraction += (_, e) => OnDragInteraction(e.Touches[0]);
        EndInteraction += (_, _) => OnEndInteraction();
    }

    // ------------------------------------------------------------
    // Handler
    // ------------------------------------------------------------

    private static void OnPropertyChanged(BindableObject bindable, object oldValue, object newValue)
    {
        ((SoundSlider)bindable).Invalidate();
    }

    private static void OnValueChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var slider = (SoundSlider)bindable;
        var value = (double)newValue;

        slider.ValueChanged?.Invoke(slider, new SoundSliderValueChangedEventArgs(value));

        if (slider.ValueChangedCommand?.CanExecute(value) == true)
        {
            slider.ValueChangedCommand.Execute(value);
        }

        slider.Invalidate();
    }

    // ------------------------------------------------------------
    // Update
    // ------------------------------------------------------------

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

    // ------------------------------------------------------------
    // Helper
    // ------------------------------------------------------------

    private bool IsPointInThumb(PointF point)
    {
        const double thumbTouchMargin = 12.0;

        var thumbX = Width / 2;

        var valuePercentage = (Value - Minimum) / (Maximum - Minimum);
        var trackTop = ThumbHeight / 2;
        var trackBottom = Height - (ThumbHeight / 2);
        var trackHeight = trackBottom - trackTop;
        var thumbY = (float)(trackBottom - (valuePercentage * trackHeight));

        var thumbRect = new RectF(
            (float)(thumbX - (ThumbWidth / 2) - thumbTouchMargin),
            (float)(thumbY - (ThumbHeight / 2) - thumbTouchMargin),
            (float)(ThumbWidth + (thumbTouchMargin * 2)),
            (float)(ThumbHeight + (thumbTouchMargin * 2)));

        return thumbRect.Contains(point);
    }

    // ------------------------------------------------------------
    // Draw
    // ------------------------------------------------------------

    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        var thumbX = dirtyRect.Width / 2;

        var valuePercentage = (Value - Minimum) / (Maximum - Minimum);

        var trackTop = (float)(ThumbHeight / 2);
        var trackBottom = dirtyRect.Height - (float)(ThumbHeight / 2);
        var trackHeight = trackBottom - trackTop;

        var thumbY = (float)(trackBottom - (valuePercentage * trackHeight));

        // Tick
        if (HasTickMarks && TickMarkCount > 1)
        {
            canvas.StrokeColor = TickMarkColor;
            canvas.StrokeSize = TickWidth;

            for (var i = 0; i < TickMarkCount; i++)
            {
                var tickPercentage = (float)i / (TickMarkCount - 1);
                var tickY = trackBottom - (tickPercentage * trackHeight);
                canvas.DrawLine(thumbX - TickMarkLength, tickY, thumbX + TickMarkLength, tickY);
            }
        }

        // Track
        canvas.StrokeSize = (float)TrackWidth;
        canvas.StrokeColor = TrackColor;
        canvas.DrawLine(thumbX, trackTop - TickWidth, thumbX, trackBottom + TickWidth);

        canvas.StrokeColor = ProgressColor;
        canvas.DrawLine(thumbX, thumbY - TickWidth, thumbX, trackBottom + TickWidth);

        // Thumb
        var thumbHalfWidth = (float)(ThumbWidth / 2);
        var thumbHalfHeight = (float)(ThumbHeight / 2);

        var thumbRect = new RectF(thumbX - thumbHalfWidth, thumbY - thumbHalfHeight, (float)ThumbWidth, (float)ThumbHeight);

        canvas.FillColor = ThumbColor1;
        canvas.FillRectangle(new RectF(thumbRect.X, thumbRect.Y, thumbRect.Width, thumbRect.Height * 0.25f));

        canvas.FillColor = ThumbColor2;
        canvas.FillRectangle(new RectF(thumbRect.X, thumbRect.Y + thumbRect.Height * 0.75f, thumbRect.Width, thumbRect.Height * 0.25f));

        var gradientBrush = new LinearGradientBrush
        {
            StartPoint = new Point(0, 0),
            EndPoint = new Point(0, 1)
        };
        gradientBrush.GradientStops.Add(new GradientStop(ThumbColor2, 0));
        gradientBrush.GradientStops.Add(new GradientStop(ThumbColor1, 1));

        var thumbMiddleRect = new RectF(thumbRect.X, thumbRect.Y + thumbRect.Height * 0.25f, thumbRect.Width, thumbRect.Height * 0.5f);
        canvas.SetFillPaint(gradientBrush, thumbMiddleRect);
        canvas.FillRectangle(thumbMiddleRect);
    }
}
