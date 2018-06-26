namespace Example.FormsApp.Modules.Devices
{
    using System.Threading.Tasks;

    using Smart.Forms.Input;
    using Smart.Navigation;

    public class DeviceMenuViewModel : AppViewModelBase
    {
        public AsyncCommand<ViewId> ForwardCommand { get; }

        public DeviceMenuViewModel(ApplicationState applicationState)
            : base(applicationState)
        {
            ForwardCommand = MakeAsyncCommand<ViewId>(x => Navigator.ForwardAsync(x));
        }

        protected override Task OnNotifyBackAsync()
        {
            return Navigator.ForwardAsync(ViewId.Menu);
        }
    }
}
