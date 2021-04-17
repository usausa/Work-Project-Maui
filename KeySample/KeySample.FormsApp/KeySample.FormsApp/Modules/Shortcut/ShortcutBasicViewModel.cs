namespace KeySample.FormsApp.Modules.Shortcut
{
    using System.Threading.Tasks;
    using System.Windows.Input;

    using Smart.Navigation;

    public class ShortcutBasicViewModel : AppViewModelBase
    {
        public ICommand BackCommand { get; }

        public ShortcutBasicViewModel(
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
