namespace DeviceStatusExample;

public partial class App
{
	public App()
	{
		InitializeComponent();

		MainPage = new MainPage();
	}

    protected override async void OnStart()
    {
        var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
        if (status != PermissionStatus.Granted)
        {
            await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
        }
    }
}
