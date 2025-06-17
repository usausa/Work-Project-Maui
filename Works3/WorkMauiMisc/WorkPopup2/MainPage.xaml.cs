namespace WorkPopup2;

using System.Diagnostics;

using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Extensions;

using Microsoft.Maui.Controls.Shapes;

public partial class MainPage : ContentPage
{
    private IPopupService popupService;

    public MainPage(IPopupService popupService)
    {
        InitializeComponent();

        this.popupService = popupService;
    }

    // ReSharper disable once AsyncVoidMethod
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

    // ReSharper disable once AsyncVoidMethod
    private async void OnTest2Click(object? sender, EventArgs e)
    {
        var result = await this.ShowPopupAsync<int>(
            new InputDialog(),
            new PopupOptions
            {
                CanBeDismissedByTappingOutsideOfPopup = false,
                //Color PageOverlayColor
                //Action? OnTappingOutsideOfPopup
                Shape = null,
                Shadow = null
            });

        Debug.WriteLine(result.Result);
    }
}
