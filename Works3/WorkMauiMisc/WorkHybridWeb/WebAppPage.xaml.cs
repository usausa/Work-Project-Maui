namespace WorkHybridWeb;

public partial class WebAppPage : ContentPage
{
    public WebAppPage()
    {
        InitializeComponent();
    }

    private void HybridWebView_OnRawMessageReceived(object? sender, HybridWebViewRawMessageReceivedEventArgs e)
    {
    }
}
