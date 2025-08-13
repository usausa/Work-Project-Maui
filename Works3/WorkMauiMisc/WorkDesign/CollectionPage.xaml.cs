using System.Diagnostics;
using Smart.Maui.Input;

namespace WorkDesign;

using System.Collections.ObjectModel;

using Smart.Maui.ViewModels;

public partial class CollectionPage : ContentPage
{
	public CollectionPage()
	{
		InitializeComponent();
	}
}

public sealed class CollectionPageViewModel : ExtendViewModelBase
{
    public ObservableCollection<CollectionData> Items { get; } = new();

    //public ObservableCollection<object> SelectedItems { get; set; } = [];
    public List<object> SelectedItems { get; set; } = [];

    public IObserveCommand SelectedCommand { get; }

    public CollectionPageViewModel()
	{
		Items.Add(new CollectionData { Id = 1, Name = "Data-1" });
		Items.Add(new CollectionData { Id = 2, Name = "Data-2" });
		Items.Add(new CollectionData { Id = 3, Name = "Data-3" });
		Items.Add(new CollectionData { Id = 4, Name = "Data-4" });
		Items.Add(new CollectionData { Id = 5, Name = "Data-5" });

        SelectedCommand = MakeDelegateCommand(() =>
        {
            Debug.WriteLine($"Changed. count={SelectedItems?.Count}");
        });
    }
}

public sealed class CollectionData
{
	public int Id { get; set; }

	public string Name { get; set; } = default!;
}