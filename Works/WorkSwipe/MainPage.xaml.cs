namespace WorkSwipe;

using System.Diagnostics;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
    }

    private void SwipeGestureRecognizer_Swiped(object sender, SwipedEventArgs e)
    {
        Debug.WriteLine("* Swiped");
    }
}

