using System.Diagnostics;

namespace WorkDesign;

using Smart.Maui.ViewModels;
using Smart.Mvvm;

using System.Windows.Input;

public partial class BasicSwipePage : ContentPage
{
    private BasicSwipePageViewModel TypedContext => (BasicSwipePageViewModel)BindingContext;

    public BasicSwipePage()
	{
		InitializeComponent();
	}
}

public sealed partial class BasicSwipePageViewModel : ExtendViewModelBase
{
    [ObservableProperty]
    public partial List<BasicEntity> Results { get; set; } = [];

    public ICommand DeleteCommand { get; }

    public BasicSwipePageViewModel()
    {
        Results = BasicService.GetData().ToList();
        DeleteCommand = MakeDelegateCommand<BasicEntity>(x =>
        {
            Debug.WriteLine($"* {x.Id} {x.Group} {x.Name}");
        });
    }
}