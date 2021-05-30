namespace KeySample.FormsApp.Modules.Shortcut
{
    using System.Threading.Tasks;

    using Smart.Navigation;

    public class ShortcutStack1ViewModel : AppViewModelBase
    {
        public ShortcutStack1ViewModel(
            ApplicationState applicationState)
            : base(applicationState)
        {
        }

        protected override Task OnNotifyBackAsync() => Navigator.ForwardAsync(ViewId.Menu);

        protected override Task OnNotifyFunction1() => OnNotifyBackAsync();
    }
}
