namespace WorkDesign;

using System.Globalization;
using Smart.Maui.Input;
using Smart.Maui.ViewModels;
using Smart.Mvvm;

public partial class LottiePage : ContentPage
{
	public LottiePage()
	{
		InitializeComponent();
	}
}

public sealed partial class LottiePageViewModel : ExtendViewModelBase
{
	[ObservableProperty]
    public partial bool IsAnimationEnabled { get; set; }

    [ObservableProperty]
    public partial TimeSpan Duration { get; set; }

    [ObservableProperty]
    public partial TimeSpan Progress { get; set; }

    public IObserveCommand ResetCommand { get; }

    public IObserveCommand PlayCommand { get; }

	public LottiePageViewModel()
    {
        ResetCommand = MakeDelegateCommand(() => Progress = TimeSpan.Zero);
        PlayCommand = MakeDelegateCommand(() => IsAnimationEnabled = !IsAnimationEnabled);
    }
}

public sealed class TimeSpanToDoubleConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        value switch
        {
            TimeSpan ts => ts.TotalMilliseconds == 0 && parameter is not null
                ? double.Parse(parameter.ToString()!)
                : ts.TotalMilliseconds,
            _ => throw new ArgumentException("Value was not a TimeSpan.", nameof(value)),
        };
    //{
    //    if (value is TimeSpan ts)
    //    {
    //        return ts.TotalMilliseconds == 0 ? 1d : ts.TotalMilliseconds;
    //    }

    //    return 1d;
    //}

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is double d)
        {
            return TimeSpan.FromSeconds(d);
        }

        return TimeSpan.Zero;
    }
}