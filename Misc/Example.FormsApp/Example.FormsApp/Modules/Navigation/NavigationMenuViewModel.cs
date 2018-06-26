namespace Example.FormsApp.Modules.Navigation
{
    using System.Threading.Tasks;

    using Smart.Forms.Input;
    using Smart.Navigation;

    public class NavigationMenuViewModel : AppViewModelBase
    {
        public AsyncCommand<ViewId> ForwardCommand { get; }

        public NavigationMenuViewModel(ApplicationState applicationState)
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