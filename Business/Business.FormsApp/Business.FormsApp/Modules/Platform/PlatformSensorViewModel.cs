namespace Business.FormsApp.Modules.Platform
{
    using System.Threading.Tasks;

    using Business.FormsApp.Components.Sensor;

    using Smart.ComponentModel;
    using Smart.Forms.Input;
    using Smart.Navigation;

    public class PlatformSensorViewModel : AppViewModelBase
    {
        public static PlatformSensorViewModel DesignInstance { get; } = null; // For design

        private readonly ISensors sensors;

        public NotificationValue<string> Location { get; } = new NotificationValue<string>();

        public AsyncCommand GetLocationCommand { get; }

        public PlatformSensorViewModel(
            ApplicationState applicationState,
            ISensors sensors)
            : base(applicationState)
        {
            this.sensors = sensors;

            GetLocationCommand = MakeAsyncCommand(GetLocation);
        }

        protected override Task OnNotifyBackAsync()
        {
            return Navigator.ForwardAsync(ViewId.PlatformMenu);
        }

        private async Task GetLocation()
        {
            var location = await sensors.GetLastLocationAsync();
            Location.Value = location != null ? $"{location.Latitude}, {location.Longitude}" : "Failed";
        }
    }
}
