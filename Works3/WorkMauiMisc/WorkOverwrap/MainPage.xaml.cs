using System.Diagnostics;

namespace WorkOverwrap;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
    }

    private void Button_OnClicked(object? sender, EventArgs e)
    {
        Debug.WriteLine("****");
    }
}
