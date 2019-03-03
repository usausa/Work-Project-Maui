namespace Business.FormsApp.Modules.Platform
{
    using System.Threading.Tasks;

    using Smart.Forms.Input;
    using Smart.Navigation;

    public class PlatformMenuViewModel : AppViewModelBase
    {
        public static PlatformMenuViewModel DesignInstance { get; } = null; // For design

        public AsyncCommand<ViewId> ForwardCommand { get; }

        public PlatformMenuViewModel(ApplicationState applicationState)
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
