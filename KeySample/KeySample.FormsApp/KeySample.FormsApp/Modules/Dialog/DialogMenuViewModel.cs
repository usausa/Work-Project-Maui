namespace KeySample.FormsApp.Modules.Dialog
{
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows.Input;

    using KeySample.FormsApp.Components.Dialog;

    using Smart.Navigation;

    public class DialogMenuViewModel : AppViewModelBase
    {
        private int selected = -1;

        public ICommand InformationCommand { get; }
        public ICommand ConfirmCommand { get; }
        public ICommand SelectCommand { get; }
        public ICommand BackCommand { get; }

        public DialogMenuViewModel(
            ApplicationState applicationState,
            IApplicationDialog dialog)
            : base(applicationState)
        {
            InformationCommand = MakeAsyncCommand(async () =>
            {
                await dialog.Information("message");
            });
            ConfirmCommand = MakeAsyncCommand(async () =>
            {
                var ret = await dialog.Confirm("message");
                await dialog.Information($"result=[{ret}]");
            });
            SelectCommand = MakeAsyncCommand(async () =>
            {
                selected = await dialog.Select(Enumerable.Range(1, 15).Select(x => $"Item-{x}").ToArray(), selected);
                await dialog.Information($"result=[{selected}]");
            });
            BackCommand = MakeAsyncCommand(OnNotifyBackAsync);
        }

        protected override Task OnNotifyBackAsync()
        {
            return Navigator.ForwardAsync(ViewId.Menu);
        }
    }
}
