namespace WorkDesign;

using Fonts;

using Smart.Maui.ViewModels;
using Smart.Mvvm;

public partial class AsyncLoadPage : ContentPage
{
	public AsyncLoadPage()
	{
		InitializeComponent();
	}
}

public sealed partial class AsyncLoadPageViewModel : ExtendViewModelBase
{
	[ObservableProperty]
	public partial bool IsLoading { get; set; }

    [ObservableProperty]
    public partial ImageSource Source { get; set; }

    public AsyncLoadPageViewModel()
    {
        Source = new FontImageSource
        {
            FontFamily = "MaterialIcons",
            Glyph = MaterialIcons.Error,
            Size = 64,
            Color = Colors.White,
        };

        _ = Task.Run(async () =>
        {
            IsLoading = true;

            await Task.Delay(3000);

            Source = new FontImageSource
            {
                FontFamily = "MaterialIcons",
                Glyph = MaterialIcons.Star,
                Size = 64,
                Color = Colors.Orange
            };

            IsLoading = false;
        });
    }
}