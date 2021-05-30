namespace KeySample.FormsApp.Modules.Control
{
    using System.Threading.Tasks;

    using Smart.Navigation;

    public class ControlListViewModel : AppViewModelBase
    {
        public ControlListViewModel(
            ApplicationState applicationState)
            : base(applicationState)
        {
        }

        protected override Task OnNotifyBackAsync() => Navigator.ForwardAsync(ViewId.Menu);

        protected override Task OnNotifyFunction1() => OnNotifyBackAsync();
    }
}
