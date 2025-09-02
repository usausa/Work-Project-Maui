namespace WorkNavAnime;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
    }

    private void OnPage1Clicked(object? sender, EventArgs e)
    {
        Navigate(new Page1());
    }

    private void OnPage2Clicked(object? sender, EventArgs e)
    {
        Navigate(new Page2());
    }

    private void Navigate(ContentView page)
    {
        Container.Children.Clear();
        Container.Children.Add(page);
    }
}
