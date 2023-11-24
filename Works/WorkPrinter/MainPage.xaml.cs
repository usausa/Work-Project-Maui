namespace WorkPrinter;

public partial class MainPage
{
    public MainPage()
    {
        InitializeComponent();
    }

    private async void Button_OnClicked(object sender, EventArgs e)
    {
        var result = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
        if (result != PermissionStatus.Granted)
        {
            return;
        }

        // TODO
    }
}
