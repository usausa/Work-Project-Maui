namespace WorkDesign;

public partial class UsaTestPage : ContentPage
{
	public UsaTestPage()
	{
		InitializeComponent();

        CollectionView.ItemsSource = Enumerable.Range(1, 10).Select(x => new { Name = $"Item {x}" }).ToList();
    }
}