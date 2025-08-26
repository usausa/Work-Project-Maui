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