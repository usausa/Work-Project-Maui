namespace WorkNfc;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
    }

    private void OnStartClicked(object? sender, EventArgs e)
    {
    }

    private void OnStopClicked(object? sender, EventArgs e)
    {
    }
}

public partial class NfcReader
{
    // TODO
}

#if ANDROID
public partial class NfcReader
{
    // TODO
}
#endif
