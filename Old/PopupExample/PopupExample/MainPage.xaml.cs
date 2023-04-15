namespace PopupExample;

using System.Diagnostics;

using CommunityToolkit.Maui.Views;

public partial class MainPage
{
    public MainPage()
    {
        InitializeComponent();
    }

    private async void Button_OnClicked(object? sender, EventArgs e)
    {
        Debug.WriteLine($"Main Width: {Width}");
        Debug.WriteLine($"Main Height: {Height}");
        await this.ShowPopupAsync(new ChildPopup());
    }
}