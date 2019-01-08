namespace Business.FormsApp.Modules.Dialog
{
    using System.Threading.Tasks;

    using Smart.Forms.Input;
    using Smart.Navigation;

    public class DialogMenuViewModel : AppViewModelBase
    {
        public AsyncCommand<ViewId> ForwardCommand { get; }

        public DialogMenuViewModel(ApplicationState applicationState)
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
