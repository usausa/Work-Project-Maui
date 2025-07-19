namespace WorkHybridWeb;

using Microsoft.Maui.Controls;

public partial class WebBasicPage : ContentPage
{
	public WebBasicPage()
	{
		InitializeComponent();
    }

    private void HybridWebView_OnRawMessageReceived(object? sender, HybridWebViewRawMessageReceivedEventArgs e)
    {
    }
}
