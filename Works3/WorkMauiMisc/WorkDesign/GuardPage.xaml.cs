namespace WorkDesign;

using Smart.Maui.ViewModels;
using Smart.Mvvm;

using System.Diagnostics;
using System.Globalization;
using System.Windows.Input;

public partial class GuardPage : ContentPage
{
	public GuardPage()
	{
		InitializeComponent();
	}
}

public sealed partial class GuardPageViewMode : ExtendViewModelBase
{
	[ObservableProperty]
    public partial bool IsLoading { get; set; }

	public ICommand TestCommand { get; }

    public ICommand LoadCommand { get; }

    public GuardPageViewMode()
    {
        TestCommand = MakeDelegateCommand(() => Debug.WriteLine("*"));
        LoadCommand = MakeAsyncCommand(async () =>
        {
            IsLoading = true;
            await Task.Delay(3000);
            IsLoading = false;
        });
    }
}

public sealed class ReverseConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value is bool boolValue ? !boolValue : value;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value is bool boolValue ? !boolValue : value;
    }
}
