using System.Diagnostics;
using System.Windows.Input;
using Smart.Maui.ViewModels;
using Smart.Mvvm;

namespace WorkDesign;

public partial class BasicSearchPage : ContentPage
{
	public BasicSearchPage()
	{
		InitializeComponent();
    }

    private void IOnTextChanged(object? sender, TextChangedEventArgs e)
    {
        Debug.WriteLine(((SearchBar)sender!).Text);
    }
}

public sealed partial class BasicSearchPageViewModel : ExtendViewModelBase
{
    [ObservableProperty]
    public partial List<BasicEntity> Results { get; set; } = [];

    public ICommand SearchCommand { get; }

    public BasicSearchPageViewModel()
    {
        SearchCommand = MakeDelegateCommand<string>(x =>
        {
            var list = BasicService.GetData();
            if (!String.IsNullOrEmpty(x))
            {
                list = list.Where(y => y.Name.Contains(x));
            }

            Results = list.ToList();
        });
    }
}