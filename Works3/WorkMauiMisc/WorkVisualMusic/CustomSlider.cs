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
        propertyChanged: OnValueChanged, defaultBindingMode: BindingMode.TwoWay,
        coerceValue: CoerceValue);

    public double Value
    {
        get => (double)GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
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

    public event EventHandler<CustomSliderValueChangedEventArgs>? ValueChanged;

    public CustomSlider()
    {
        Drawable = this;

        StartInteraction += (sender, args) =>
        {

        };
        //DragInteraction += OnDragInteraction;
        //EndInteraction += OnEndInteraction;
    }

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

    private static object CoerceValue(BindableObject bindable, object value)
    {
        var slider = (CustomSlider)bindable;
        var doubleValue = (double)value;

        if (doubleValue < slider.Minimum)
            return slider.Minimum;

        if (doubleValue > slider.Maximum)
            return slider.Maximum;

        return doubleValue;
    }

    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
    }
}
