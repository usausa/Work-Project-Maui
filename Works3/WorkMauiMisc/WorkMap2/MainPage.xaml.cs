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
        MapView.Map.Home = n => n.CenterOnAndZoomTo(sphericalMercatorCoordinate, n.Resolutions[9]);
    }

    private void HomeClicked(object? sender, EventArgs e)
    {
        var sphericalMercatorCoordinate = SphericalMercator.FromLonLat(InitialLongitude, InitialLatitude).ToMPoint();
        var resolutions = MapView.Map.Navigator.Resolutions;
        MapView.Map.Navigator.CenterOnAndZoomTo(sphericalMercatorCoordinate, resolutions[9]);
    }

    private void TestClicked(object? sender, EventArgs e)
    {
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