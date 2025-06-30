namespace WorkDebug;

using System.Diagnostics;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
    }

    private void Button_OnClicked(object? sender, EventArgs e)
    {
        Debug.WriteLine("* Clicked");
    }
}
