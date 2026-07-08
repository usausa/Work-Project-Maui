namespace Template.MobileApp.Messaging;

using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;

public interface IMapController
{
    void Attach(Map view);

    void Detach();
}

public sealed class MapController : IMapController
{
    private readonly Location homeLocation;

    private readonly Distance homeRadius;

    private Map? map;

    public MapController(double latitude, double longitude, double radiusKilometers)
    {
        homeLocation = new Location(latitude, longitude);
        homeRadius = Distance.FromKilometers(radiusKilometers);
    }

    void IMapController.Attach(Map view)
    {
        map = view;
        map.MoveToRegion(MapSpan.FromCenterAndRadius(homeLocation, homeRadius));
    }

    void IMapController.Detach()
    {
        map = null;
    }

    public void MoveToHome()
    {
        map?.MoveToRegion(MapSpan.FromCenterAndRadius(homeLocation, homeRadius));
    }

    public void MoveTo(double latitude, double longitude, double radiusKilometers = 1d)
    {
        map?.MoveToRegion(MapSpan.FromCenterAndRadius(new Location(latitude, longitude), Distance.FromKilometers(radiusKilometers)));
    }
}
