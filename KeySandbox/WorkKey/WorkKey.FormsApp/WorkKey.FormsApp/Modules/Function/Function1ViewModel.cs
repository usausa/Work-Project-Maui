namespace WorkKey.FormsApp.Modules.Function
{
    using System.Threading.Tasks;

    using Smart.Navigation;

    using WorkKey.FormsApp.Components.Dialog;

    public class Function1ViewModel : AppViewModelBase
    {
        private readonly IApplicationDialog dialog;

        public Function1ViewModel(
            ApplicationState applicationState,
            IApplicationDialog dialog)
            : base(applicationState)
        {
            this.dialog = dialog;
        }

        protected override Task OnNotifyBackAsync()
        {
            return Navigator.ForwardAsync(ViewId.Menu);
        }

        protected override Task OnNotifyFunction1Async()
        {
            return OnNotifyBackAsync();
        }

        protected override async Task OnNotifyFunction4Async()
        {
            await dialog.Information("Alert");
        }
    }
}
