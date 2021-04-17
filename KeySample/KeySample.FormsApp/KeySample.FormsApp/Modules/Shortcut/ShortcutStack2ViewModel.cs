namespace KeySample.FormsApp.Modules.Shortcut
{
    using System.Threading.Tasks;
    using System.Windows.Input;

    using Smart.Navigation;

    public class ShortcutStack2ViewModel : AppViewModelBase
    {
        public ICommand BackCommand { get; }

        public ShortcutStack2ViewModel(
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
