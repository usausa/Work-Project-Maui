namespace Baseline.FormsApp.Components.Location
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Reactive.Subjects;
    using System.Threading;
    using System.Threading.Tasks;

    using Xamarin.Essentials;

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Ignore")]
    public sealed class LocationManager : ILocationManager
    {
        private readonly Subject<LocationInformation> locationInformation = new Subject<LocationInformation>();

        private bool running;

        private CancellationTokenSource cts;

        public IObservable<LocationInformation> LocationInformation => locationInformation;

        public int Interval { get; set; } = 10000;

        public void Start()
        {
            if (running)
            {
                return;
            }

            running = true;

            cts = new CancellationTokenSource();
            Task.Run(() => GetLocationLoop(cts));
        }

        public void Stop()
        {
            if (!running)
            {
                return;
            }

            running = false;

            if (cts != null)
            {
                cts.Cancel();
                cts.Dispose();
                cts = null;
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ignore")]
        private async Task GetLocationLoop(CancellationTokenSource cancellationTokenSource)
        {
            try
            {
                while (!cancellationTokenSource.IsCancellationRequested)
                {
                    var request = new GeolocationRequest(GeolocationAccuracy.Medium);
                    var location = await Geolocation.GetLocationAsync(request, cancellationTokenSource.Token);
                    if (location != null)
                    {
                        locationInformation.OnNext(new LocationInformation(location.Latitude, location.Longitude, location.Timestamp));
                    }

                    await Task.Delay(Interval, cancellationTokenSource.Token);
                }
            }
            catch (Exception e)
            {
                Trace.WriteLine(e);
                locationInformation.OnNext(null);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ignore")]
        public async Task<LocationInformation> GetLastLocationAsync()
        {
            try
            {
                var location = await Geolocation.GetLastKnownLocationAsync();
                if (location != null)
                {
                    return new LocationInformation(location.Latitude, location.Longitude, location.Timestamp);
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);
            }

            return null;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ignore")]
        public async Task<LocationInformation> GetLocationAsync(CancellationTokenSource cancellationTokenSource)
        {
            try
            {
                var request = new GeolocationRequest(GeolocationAccuracy.Medium);
                var location = await Geolocation.GetLocationAsync(request, cancellationTokenSource.Token);
                if (location != null)
                {
                    return new LocationInformation(location.Latitude, location.Longitude, location.Timestamp);
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);
            }

            return null;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ignore")]
        public async Task<PlaceInformation[]> GetPlaceInformationAsync(double latitude, double longitude)
        {
            try
            {
                var placemarks = await Geocoding.GetPlacemarksAsync(latitude, longitude);
                return placemarks.Select(x => new PlaceInformation(x)).ToArray();
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);
            }

            return Array.Empty<PlaceInformation>();
        }
    }
}
