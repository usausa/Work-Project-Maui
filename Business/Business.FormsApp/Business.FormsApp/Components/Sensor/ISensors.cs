namespace Business.FormsApp.Components.Sensor
{
    using System.Threading.Tasks;

    public interface ISensors
    {
        Task<LocationResult> GetLastLocationAsync();
    }
}
