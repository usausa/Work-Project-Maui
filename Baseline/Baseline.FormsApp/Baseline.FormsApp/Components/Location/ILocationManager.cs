namespace Baseline.FormsApp.Components.Location
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    public interface ILocationManager
    {
        IObservable<LocationInformation> LocationInformation { get; }

        int Interval { get; set; }

        void Start();

        void Stop();

        Task<LocationInformation> GetLastLocationAsync();

        Task<LocationInformation> GetLocationAsync(CancellationTokenSource cancellationTokenSource);

        Task<PlaceInformation[]> GetPlaceInformationAsync(double latitude, double longitude);
    }
}
