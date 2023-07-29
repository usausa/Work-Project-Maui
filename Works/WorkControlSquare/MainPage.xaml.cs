namespace WorkControlSquare;

using System.Diagnostics;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
    }

    //private void Button_OnClicked(object sender, EventArgs e)
    //{
    //    BoxView1.HeightRequest = Grid1.Width;
    //}
    private void Button_OnClicked(object sender, EventArgs e)
    {
        Debug.WriteLine($"Height={BoxView.Height}, HeightRequest={BoxView.HeightRequest}");
    }
}

