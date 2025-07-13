using System.Diagnostics;

namespace WorkDesign;

public partial class BadgePage : ContentPage
{
	public BadgePage()
	{
		InitializeComponent();
	}

    private void Button_OnClicked(object? sender, EventArgs e)
    {
		Debug.WriteLine("*");
    }
}