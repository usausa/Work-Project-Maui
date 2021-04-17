namespace KeySample.FormsApp.Modules.Shortcut
{
    using System.Threading.Tasks;
    using System.Windows.Input;

    using Smart.Navigation;

    public class ShortcutStack1ViewModel : AppViewModelBase
    {
        public ICommand BackCommand { get; }

        public ShortcutStack1ViewModel(
            ApplicationState applicationState)
            : base(applicationState)
        {
            BackCommand = MakeAsyncCommand(OnNotifyBackAsync);
        }

        protected override Task OnNotifyBackAsync()
        {
            return Navigator.ForwardAsync(ViewId.Menu);
        }
    }
}
