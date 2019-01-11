namespace Business.FormsApp.Components.Sensor
{
    public class LocationResult
    {
        public double Latitude { get; }

        public double Longitude { get; }

        public LocationResult(double latitude, double longitude)
        {
            Latitude = latitude;
            Longitude = longitude;
        }
    }
}
