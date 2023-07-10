namespace WorkNativeControl;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
    }

    protected override bool OnBackButtonPressed()
    {
        EventHub.Default.Handle(null, EventArgs.Empty);
        return true;
    }
}

