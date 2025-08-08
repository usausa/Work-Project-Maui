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

    public CollectionPageViewModel()
	{
		Items.Add(new CollectionData { Id = 1, Name = "Data-1" });
		Items.Add(new CollectionData { Id = 2, Name = "Data-2" });
		Items.Add(new CollectionData { Id = 3, Name = "Data-3" });
		Items.Add(new CollectionData { Id = 4, Name = "Data-4" });
		Items.Add(new CollectionData { Id = 5, Name = "Data-5" });
    }
}

public sealed class CollectionData
{
	public int Id { get; set; }

	public string Name { get; set; } = default!;
}