using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Extensions;

namespace WorkPopup2;

using Microsoft.Maui.Controls.Shapes;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
    }

    private async void OnTest1Click(object? sender, EventArgs e)
    {
        await this.ShowPopupAsync(new Label
        {
            Text = "This is a very important message!"
        }, new PopupOptions
        {
            CanBeDismissedByTappingOutsideOfPopup = true,
            Shape = new RoundRectangle
            {
                CornerRadius = new CornerRadius(20, 20, 20, 20),
                StrokeThickness = 2,
                Stroke = Colors.LightGray
            }
        });
    }
}
