namespace PopupExample;

using CommunityToolkit.Maui.Views;

public partial class MainPage
{
    public MainPage()
    {
        InitializeComponent();
    }

    private async void Button_OnClicked(object? sender, EventArgs e)
    {
        await this.ShowPopupAsync(new ChildPopup());
    }
}