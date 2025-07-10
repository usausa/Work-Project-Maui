using System.Diagnostics;

namespace WorkOverwrap;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
    }

    private async void Button_OnClicked(object? sender, EventArgs e)
    {
        Debug.WriteLine("****");

        OverwrapLayout.OverwrapVisible = true;

        await Task.Delay(2000);

        OverwrapLayout.OverwrapVisible = false;
    }
}
