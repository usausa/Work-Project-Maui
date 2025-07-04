using Mapsui.Extensions;

namespace WorkMap2;

using Mapsui.Projections;
using Mapsui.UI.Maui;

using Smart.Maui.Interactivity;

using System.ComponentModel;

public sealed class MoveToEventArgs : EventArgs
{
    public double Longitude { get; set; }

    public double Latitude { get; set; }

    public int? Resolution { get; set; }
}


internal class MapsuiController
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public event EventHandler<MoveToEventArgs>? MoveToRequest;

    [EditorBrowsable(EditorBrowsableState.Never)]
    public event EventHandler<EventArgs>? ZoomInRequest;

    [EditorBrowsable(EditorBrowsableState.Never)]
    public event EventHandler<EventArgs>? ZoomOutRequest;

    public double HomeLongitude { get; }

    public double HomeLatitude { get; }

    public int? InitialResolution { get; }

    public MapsuiController(double homeLongitude, double homeLatitude, int? initialResolution = null)
    {
        HomeLongitude = homeLongitude;
        HomeLatitude = homeLatitude;
        InitialResolution = initialResolution;
    }

    public void MoveTo(double longitude, double latitude, int? resolution = null)
    {
        var args = new MoveToEventArgs
        {
            Longitude = longitude,
            Latitude = latitude,
            Resolution = resolution
        };
        MoveToRequest?.Invoke(this, args);
    }

    public void ZoomIn()
    {
        ZoomInRequest?.Invoke(this, EventArgs.Empty);
    }

    public void ZoomOut()
    {
        ZoomOutRequest?.Invoke(this, EventArgs.Empty);
    }
}

internal class MapsuiBind
{
    public static readonly BindableProperty ControllerProperty = BindableProperty.CreateAttached(
        "Controller",
        typeof(MapsuiController),
        typeof(MapsuiBind),
        null,
        propertyChanged: BindChanged);

    public static MapsuiController? GetController(BindableObject bindable) =>
        (MapsuiController)bindable.GetValue(ControllerProperty);

    public static void SetController(BindableObject bindable, MapsuiController? value) =>
        bindable.SetValue(ControllerProperty, value);

    private static void BindChanged(BindableObject bindable, object? oldValue, object? newValue)
    {
        if (bindable is not MapControl view)
        {
            return;
        }

        if (oldValue is not null)
        {
            var behavior = view.Behaviors.FirstOrDefault(static x => x is MapsuiBindBehavior);
            if (behavior is not null)
            {
                view.Behaviors.Remove(behavior);
            }
        }

        if (newValue is not null)
        {
            view.Behaviors.Add(new MapsuiBindBehavior());
        }
    }

    private sealed class MapsuiBindBehavior : BehaviorBase<MapControl>
    {
        private MapsuiController? controller;

        protected override void OnAttachedTo(MapControl bindable)
        {
            base.OnAttachedTo(bindable);

            controller = GetController(bindable);
            if ((controller is not null) && (AssociatedObject is not null))
            {

                AssociatedObject.Map.Layers.Add(Mapsui.Tiling.OpenStreetMap.CreateTileLayer());

                var sphericalMercatorCoordinate = SphericalMercator.FromLonLat(controller.HomeLongitude, controller.HomeLatitude).ToMPoint();
                if (controller.InitialResolution.HasValue)
                {
                    AssociatedObject.Map.Home = n => n.CenterOnAndZoomTo(sphericalMercatorCoordinate, n.Resolutions[controller.InitialResolution.Value]);
                }
                else
                {
                    AssociatedObject.Map.Home = n => n.CenterOn(sphericalMercatorCoordinate);
                }

                controller.MoveToRequest += OnMoveToRequest;
                controller.ZoomInRequest += OnZoomInRequest;
                controller.ZoomOutRequest += OnZoomOutRequest;
            }
        }

        protected override void OnDetachingFrom(MapControl bindable)
        {
            if (controller is not null)
            {
                controller.MoveToRequest -= OnMoveToRequest;
                controller.ZoomInRequest -= OnZoomInRequest;
                controller.ZoomOutRequest -= OnZoomOutRequest;
            }

            controller = null;

            base.OnDetachingFrom(bindable);
        }

        private void OnMoveToRequest(object? sender, MoveToEventArgs e)
        {
            var mapControl = AssociatedObject;
            if (mapControl is null)
            {
                return;
            }

            var sphericalMercatorCoordinate = SphericalMercator.FromLonLat(e.Longitude, e.Latitude).ToMPoint();

            if (e.Resolution.HasValue)
            {
                mapControl.Map.Navigator.CenterOnAndZoomTo(sphericalMercatorCoordinate, mapControl.Map.Navigator.Resolutions[e.Resolution.Value]);
            }
            else
            {
                mapControl.Map.Navigator.CenterOn(sphericalMercatorCoordinate);
            }
        }

        private void OnZoomInRequest(object? sender, EventArgs e)
        {
            var mapControl = AssociatedObject;
            if (mapControl is null)
            {
                return;
            }

            mapControl.Map.Navigator.ZoomIn();
        }

        private void OnZoomOutRequest(object? sender, EventArgs e)
        {
            var mapControl = AssociatedObject;
            if (mapControl is null)
            {
                return;
            }

            mapControl.Map.Navigator.ZoomOut();
        }
    }
}
