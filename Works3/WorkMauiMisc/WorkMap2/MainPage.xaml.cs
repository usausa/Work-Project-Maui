namespace WorkMap2;

using Mapsui.Extensions;
using Mapsui.Projections;

public partial class MainPage : ContentPage
{
    private const double InitialLatitude = 35.6895; // 例: 東京
    private const double InitialLongitude = 139.6917; // 例: 東京

    public MainPage()
    {
        InitializeComponent();

        MapView.Map.Layers.Add(Mapsui.Tiling.OpenStreetMap.CreateTileLayer());

        var sphericalMercatorCoordinate = SphericalMercator.FromLonLat(InitialLongitude, InitialLatitude).ToMPoint();
        MapView.Map.Navigator.CenterOnAndZoomTo(sphericalMercatorCoordinate, MapView.Map.Navigator.Resolutions[9]);
    }

    private void HomeClicked(object? sender, EventArgs e)
    {
        var sphericalMercatorCoordinate = SphericalMercator.FromLonLat(InitialLongitude, InitialLatitude).ToMPoint();
        MapView.Map.Navigator.CenterOn(sphericalMercatorCoordinate);
        //var resolutions = MapView.Map.Navigator.Resolutions;
        //MapView.Map.Navigator.CenterOnAndZoomTo(sphericalMercatorCoordinate, resolutions[9]);
    }

    private async void TestClicked(object? sender, EventArgs e)
    {
        var status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
    }

    private void ZoomInClicked(object? sender, EventArgs e)
    {
        MapView.Map.Navigator.ZoomIn();
    }

    private void ZoomOutClicked(object? sender, EventArgs e)
    {
        MapView.Map.Navigator.ZoomOut();
    }
}