namespace WorkControl;

using System.Diagnostics;

public partial class MainPage : ContentPage
{
	public MainPage()
	{
		InitializeComponent();
	}

    private void SimpleViewOnClicked(object sender, EventArgs e)
    {
		Debug.WriteLine("SimpleView clicked.");
    }

    private void ColorButtonOnClicked(object sender, EventArgs e)
    {
        SimpleView.Color = SimpleView.Color.Equals(Colors.Red) ? Colors.Blue : Colors.Red;
    }

    private void CallButtonOnClicked(object sender, EventArgs e)
    {
        SimpleView.PlatformCall(DateTime.Now.Second);
    }
}

