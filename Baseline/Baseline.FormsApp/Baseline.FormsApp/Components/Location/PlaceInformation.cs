namespace Baseline.FormsApp.Components.Location
{
    using Xamarin.Essentials;

    public class PlaceInformation
    {
        private readonly Placemark placemark;

        public PlaceInformation(Placemark placemark)
        {
            this.placemark = placemark;
        }

        public string CountryCode => placemark.CountryCode;

        public string CountryName => placemark.CountryName;

        public string FeatureName => placemark.FeatureName;

        public string PostalCode => placemark.PostalCode;

        public string AdminArea => placemark.AdminArea;

        public string Locality => placemark.Locality;

        public string Thoroughfare => placemark.Thoroughfare;

        public string SubAdminArea => placemark.SubAdminArea;

        public string SubLocality => placemark.SubLocality;

        public string SubThoroughfare => placemark.SubThoroughfare;
    }
}
