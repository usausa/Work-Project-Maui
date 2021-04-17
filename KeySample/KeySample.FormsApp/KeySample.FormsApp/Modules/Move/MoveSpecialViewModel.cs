namespace KeySample.FormsApp.Modules.Move
{
    using System.Threading.Tasks;
    using System.Windows.Input;

    using Smart.Navigation;

    public class MoveSpecialViewModel : AppViewModelBase
    {
        public ICommand BackCommand { get; }

        public MoveSpecialViewModel(
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
