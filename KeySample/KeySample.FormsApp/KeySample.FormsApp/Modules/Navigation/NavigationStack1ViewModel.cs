namespace KeySample.FormsApp.Modules.Navigation
{
    using System.Threading.Tasks;

    using Smart.Navigation;

    public class NavigationStack1ViewModel : AppViewModelBase
    {
        public NavigationStack1ViewModel(
            ApplicationState applicationState)
            : base(applicationState)
        {
        }

        protected override Task OnNotifyBackAsync() => Navigator.ForwardAsync(ViewId.Menu);

        protected override Task OnNotifyFunction1() => OnNotifyBackAsync();
    }
}
