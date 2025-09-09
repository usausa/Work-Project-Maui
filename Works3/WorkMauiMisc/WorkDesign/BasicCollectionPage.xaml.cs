namespace WorkDesign;

using Smart.Maui.ViewModels;
using Smart.Mvvm;

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
    public partial List<GroupEntity> Results { get; set; } = [];

    public BasicCollectionPageViewModel()
    {
        Results = BasicService.GetData()
            .GroupBy(x => x.Group)
            .Select(g => new GroupEntity(g.Key, g))
            .ToList();
    }
}