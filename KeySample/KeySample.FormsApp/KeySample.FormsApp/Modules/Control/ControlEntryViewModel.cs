namespace KeySample.FormsApp.Modules.Control
{
    using System.Threading.Tasks;
    using System.Windows.Input;

    using Smart.Navigation;

    public class ControlEntryViewModel : AppViewModelBase
    {
        public ICommand BackCommand { get; }

        public ControlEntryViewModel(
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
