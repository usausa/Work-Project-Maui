namespace KeySample.FormsApp.Modules.Popup
{
    using System.Threading.Tasks;

    using Smart.Navigation;

    public class PopupMenuViewModel : AppViewModelBase
    {
        public PopupMenuViewModel(
            ApplicationState applicationState)
            : base(applicationState)
        {
        }

        protected override Task OnNotifyBackAsync() => Navigator.ForwardAsync(ViewId.Menu);

        protected override Task OnNotifyFunction1() => OnNotifyBackAsync();
    }
}
