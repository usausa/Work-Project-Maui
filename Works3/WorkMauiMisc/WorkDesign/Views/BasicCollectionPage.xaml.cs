using System.Windows.Input;

namespace WorkDesign;

using Smart.Maui.ViewModels;
using Smart.Mvvm;

using System.Globalization;

public partial class BasicCollectionPage : ContentPage
{
	public BasicCollectionPage()
	{
		InitializeComponent();
	}
}

public sealed partial class BasicCollectionPageViewModel : ExtendViewModelBase
{
    [ObservableProperty]
    public partial List<BasicGroup> Results { get; set; } = [];

    public ICommand ToggleCommand { get; }

    public BasicCollectionPageViewModel()
    {
        Results = BasicService.GetData()
            .GroupBy(x => x.Group)
            .Select(g => new BasicGroup(g.Key, g))
            .ToList();

        ToggleCommand = MakeDelegateCommand<BasicGroup>(g => g.IsExpanded = !g.IsExpanded);
    }
}

public class BoolToExpandCollapseConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value is true ? "▼" : "►";
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}