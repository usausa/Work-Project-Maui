using System.Diagnostics;

namespace WorkElement;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
    }

    private void OnButton1Clicked(object sender, EventArgs e)
    {
        Debug.WriteLine("----------");
        TraceFocused();
        var result = Entry1.Focus();
        Debug.WriteLine($"Result: {result}");
        TraceFocused();
    }

    private void OnButton2Clicked(object sender, EventArgs e)
    {
        Debug.WriteLine("----------");
        TraceFocused();
        var result = Label1.Focus();
        Debug.WriteLine($"Result: {result}");
        TraceFocused();
    }

    private void TraceFocused()
    {
        Debug.WriteLine($"Button1: {Button1.IsFocused}");
        Debug.WriteLine($"Button2: {Button2.IsFocused}");
        Debug.WriteLine($"Entry1: {Entry1.IsFocused}");
        Debug.WriteLine($"Label1: {Label1.IsFocused}");
    }
}

