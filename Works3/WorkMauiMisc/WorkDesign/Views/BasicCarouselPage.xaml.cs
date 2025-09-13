namespace WorkDesign;

using System.Collections.ObjectModel;

public partial class BasicCarouselPage : ContentPage
{
    public ObservableCollection<CarouselItem> Items { get; set; }

    public BasicCarouselPage()
	{
		InitializeComponent();

        Items =
        [
            new()
            {
                Title = "風景1",
                Description = "美しい海の景色",
                ImageUrl = "https://picsum.photos/500/500?random=1"
            },
            new()
            {
                Title = "風景2",
                Description = "山の風景",
                ImageUrl = "https://picsum.photos/500/500?random=2"
            },
            new()
            {
                Title = "風景3",
                Description = "街の風景",
                ImageUrl = "https://picsum.photos/500/500?random=3"
            },
            new()
            {
                Title = "風景4",
                Description = "森の風景",
                ImageUrl = "https://picsum.photos/500/500?random=4"
            },
            new()
            {
                Title = "風景5",
                Description = "川の風景",
                ImageUrl = "https://picsum.photos/500/500?random=5"
            }
        ];

        BindingContext = this;

        // IndicatorViewをCarouselViewに関連付ける
        indicatorView.IndicatorsShape = IndicatorShape.Circle;
        indicatorView.Count = Items.Count;
        indicatorView.MaximumVisible = Items.Count;
    }

    private void OnPositionChanged(object sender, PositionChangedEventArgs e)
    {
        indicatorView.Position = e.CurrentPosition;
    }
}

public class CarouselItem
{
    public string Title { get; set; } = default!;
    public string Description { get; set; } = default!;
    public string ImageUrl { get; set; } = default!;
}