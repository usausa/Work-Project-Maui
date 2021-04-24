namespace KeySample.FormsApp.Modules.Dialog
{
    using System.Diagnostics;
    using System.Threading.Tasks;
    using System.Windows.Input;

    using KeySample.FormsApp.Components.Dialog;

    using Smart.Navigation;

    public class DialogMenuViewModel : AppViewModelBase
    {
        public ICommand DialogCommand { get; }
        public ICommand BackCommand { get; }

        public DialogMenuViewModel(
            ApplicationState applicationState,
            IApplicationDialog dialog)
            : base(applicationState)
        {
            DialogCommand = MakeAsyncCommand(async () =>
            {
                var ret = await dialog.Confirm("message");
                Debug.WriteLine($"Confirm result=[{ret}]");
            });
            BackCommand = MakeAsyncCommand(OnNotifyBackAsync);
        }

        protected override Task OnNotifyBackAsync()
        {
            return Navigator.ForwardAsync(ViewId.Menu);
        }
    }
}
