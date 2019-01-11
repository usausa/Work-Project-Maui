namespace Business.FormsApp.Components.Sensor
{
    using System;
    using System.Threading.Tasks;

    using Xamarin.Essentials;

    public sealed class Sensors : ISensors
    {
        public async Task<LocationResult> GetLastLocationAsync()
        {
            try
            {
                var location = await Geolocation.GetLastKnownLocationAsync();
                if (location != null)
                {
                    return new LocationResult(location.Latitude, location.Longitude);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
            }

            return null;
        }
    }
}
