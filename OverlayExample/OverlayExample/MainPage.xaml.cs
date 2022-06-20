namespace OverlayExample;

public partial class MainPage
{
    public MainPage()
    {
        InitializeComponent();
    }

    private async void Button_OnClicked(object? sender, EventArgs e)
    {
        System.Diagnostics.Debug.WriteLine("**** Click start");

        var window = GetParentWindow();
        var loading = new LoadingOverlay(window);
        window.AddOverlay(loading);

        await Task.Delay(3000);

        window.RemoveOverlay(loading);

        System.Diagnostics.Debug.WriteLine("**** Click end");
    }
}
