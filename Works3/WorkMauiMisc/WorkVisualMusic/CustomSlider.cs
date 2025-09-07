namespace WorkVisualMusic;

using System;
using System.Windows.Input;

public class CustomSliderValueChangedEventArgs : EventArgs
{
    public double Value { get; }

    public CustomSliderValueChangedEventArgs(double value)
    {
        Value = value;
    }
}

public sealed class CustomSlider : GraphicsView, IDrawable
{
    private const double ThumbTouchMargin = 12.0;

    // ------------------------------------------------------------
    // Property
    // ------------------------------------------------------------

    // Value

    public static readonly BindableProperty MinimumProperty = BindableProperty.Create(
        nameof(Minimum),
        typeof(double),
        typeof(CustomSlider),
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
        typeof(CustomSlider),
        100.0,
        propertyChanged: OnPropertyChanged);

    public double Maximum
    {
        get => (double)GetValue(MaximumProperty);
        set => SetValue(MaximumProperty, value);
    }

    public static readonly BindableProperty ValueProperty = BindableProperty.Create(
        nameof(Value),
        typeof(double),
        typeof(CustomSlider),
        0.0,
        propertyChanged: OnValueChanged, defaultBindingMode: BindingMode.TwoWay);

    public double Value
    {
        get => (double)GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    // Color

    public static readonly BindableProperty TrackColorProperty = BindableProperty.Create(
        nameof(TrackColor),
        typeof(Color),
        typeof(CustomSlider),
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
        typeof(CustomSlider),
        Colors.OrangeRed,
        propertyChanged: OnPropertyChanged);

    public Color ProgressColor
    {
        get => (Color)GetValue(ProgressColorProperty);
        set => SetValue(ProgressColorProperty, value);
    }

    public static readonly BindableProperty ThumbColorProperty = BindableProperty.Create(
        nameof(ThumbColor),
        typeof(Color),
        typeof(CustomSlider),
        Colors.Silver,
        propertyChanged: OnPropertyChanged);

    public Color ThumbColor
    {
        get => (Color)GetValue(ThumbColorProperty);
        set => SetValue(ThumbColorProperty, value);
    }

    // Size

    public static readonly BindableProperty TrackWidthProperty = BindableProperty.Create(
        nameof(TrackWidth),
        typeof(double),
        typeof(CustomSlider),
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
        typeof(CustomSlider), 32.0,
        propertyChanged: OnPropertyChanged);

    public double ThumbWidth
    {
        get => (double)GetValue(ThumbWidthProperty);
        set => SetValue(ThumbWidthProperty, value);
    }

    public static readonly BindableProperty ThumbHeightProperty = BindableProperty.Create(
        nameof(ThumbHeight),
        typeof(double),
        typeof(CustomSlider),
        12.0,
        propertyChanged: OnPropertyChanged);

    public double ThumbHeight
    {
        get => (double)GetValue(ThumbHeightProperty);
        set => SetValue(ThumbHeightProperty, value);
    }

    // Event

    public static readonly BindableProperty ValueChangedCommandProperty = BindableProperty.Create(
        nameof(ValueChangedCommand),
        typeof(ICommand),
        typeof(CustomSlider));

    public ICommand? ValueChangedCommand
    {
        get => (ICommand)GetValue(ValueChangedCommandProperty);
        set => SetValue(ValueChangedCommandProperty, value);
    }

    // ------------------------------------------------------------
    // Field
    // ------------------------------------------------------------

    public event EventHandler<CustomSliderValueChangedEventArgs>? ValueChanged;

    private bool isDragging = false;

    // ------------------------------------------------------------
    // Constructor
    // ------------------------------------------------------------

    public CustomSlider()
    {
        Drawable = this;

        StartInteraction += (_, e) => OnStartInteraction(e.Touches[0]);
        DragInteraction += (_, e) => OnDragInteraction(e.Touches[0]);
        EndInteraction += (_, e) => OnEndInteraction();
    }

    // ------------------------------------------------------------
    // Handler
    // ------------------------------------------------------------

    private static void OnPropertyChanged(BindableObject bindable, object oldValue, object newValue)
    {
        ((CustomSlider)bindable).Invalidate();
    }

    private static void OnValueChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var slider = (CustomSlider)bindable;
        var value = (double)newValue;

        slider.ValueChanged?.Invoke(slider, new CustomSliderValueChangedEventArgs(value));

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

    }

    // ------------------------------------------------------------
    // Helper
    // ------------------------------------------------------------

    private bool IsPointInThumb(PointF point)
    {
        // TODO
        return false;
    }

    // TODO

    // ------------------------------------------------------------
    // Draw
    // ------------------------------------------------------------

    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
    }
}
