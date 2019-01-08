namespace Business.FormsApp.Modules.Platform
{
    using System.Threading.Tasks;

    using Smart.Forms.Input;
    using Smart.Navigation;

    public class PlatformSensorViewModel : AppViewModelBase
    {
        public PlatformSensorViewModel(ApplicationState applicationState)
            : base(applicationState)
        {
        }

        protected override Task OnNotifyBackAsync()
        {
            return Navigator.ForwardAsync(ViewId.PlatformMenu);
        }
    }
}
