namespace LocationExample;

public partial class MainPage
{
    private readonly ILocationManager manager = new LocationManager();

    public MainPage()
    {
        InitializeComponent();

        manager.LocationChanged += ManagerOnLocationChanged;
        manager.Interval = 0;
    }

    private void ManagerOnLocationChanged(object? sender, LocationEventArgs e)
    {
        UpdateLocation(e.Location);
    }

    private void StartButton_OnClicked(object? sender, EventArgs e)
    {
        manager.Start();
    }

    private void StopButton_OnClicked(object? sender, EventArgs e)
    {
        manager.Stop();
    }

    private async void LastButton_OnClicked(object? sender, EventArgs e)
    {
        var location = await manager.GetLastLocationAsync();
        if (location is not null)
        {
            UpdateLocation(location);
        }
    }

    private async void PlaceButton_OnClicked(object sender, EventArgs e)
    {
        if (!Double.TryParse(Latitude.Text, out var latitude) ||
            !Double.TryParse(Longitude.Text, out var longitude))
        {
            return;
        }

#pragma warning disable CA1031
        try
        {
            var place = (await Geocoding.GetPlacemarksAsync(latitude, longitude).ConfigureAwait(false)).FirstOrDefault();
            if (place is not null)
            {
                Place1.Text = place.CountryName;
                Place2.Text = place.FeatureName;
                Place3.Text = place.Locality;
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(ex);
        }
#pragma warning restore CA1031
    }

    private void UpdateLocation(Location location)
    {
        Latitude.Text = $"{location.Latitude}";
        Longitude.Text = $"{location.Longitude}";
        Timestamp.Text = $"{location.Timestamp}";
    }
}