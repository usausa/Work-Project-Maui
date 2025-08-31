namespace WorkDesign;

using Smart.Maui.ViewModels;
using Smart.Mvvm;

public partial class StickPage : ContentPage
{
	public StickPage()
	{
		InitializeComponent();
	}
}

public sealed partial class StickPageViewModel : ExtendViewModelBase
{
	[ObservableProperty]
    public partial double XValue { get; set; }

    [ObservableProperty]
    public partial double YValue { get; set; }
}

public class ColorToBrushConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, System.Globalization.CultureInfo culture)
    {
        return value is Color color ? new SolidColorBrush(color) : (object?)null;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, System.Globalization.CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

public sealed class ButtonSunkBehavior : Behavior<Button>
{
    protected override void OnAttachedTo(Button bindable)
    {
        base.OnAttachedTo(bindable);

        bindable.Pressed += OnButtonPressed;
        bindable.Released += OnButtonReleased;
    }
    protected override void OnDetachingFrom(Button bindable)
    {
        base.OnDetachingFrom(bindable);

        bindable.Pressed -= OnButtonPressed;
        bindable.Released -= OnButtonReleased;
    }

    private void OnButtonPressed(object? sender, EventArgs e)
    {
        if (sender is Button button)
        {
            // 小さく、暗く
            button.ScaleTo(0.9, 50, Easing.CubicOut);
            button.FadeTo(0.8, 50, Easing.CubicOut);
        }
    }

    private void OnButtonReleased(object? sender, EventArgs e)
    {
        if (sender is Button button)
        {
            button.ScaleTo(1.0, 100, Easing.CubicOut);
            button.FadeTo(1.0, 100, Easing.CubicOut);
        }
    }
}