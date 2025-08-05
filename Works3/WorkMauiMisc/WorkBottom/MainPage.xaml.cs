namespace WorkBottom;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
    }

    private async void Button_OnClicked(object? sender, EventArgs e)
    {
        var sheet = new MyBottomSheet();
        await sheet.ShowAsync();
    }
}
