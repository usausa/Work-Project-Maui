namespace WorkKey.FormsApp.Modules.Function
{
    using System.Threading.Tasks;
    using System.Windows.Input;

    using Smart.ComponentModel;
    using Smart.Navigation;

    using WorkKey.FormsApp.Components.Dialog;

    public class Function2ViewModel : AppViewModelBase
    {
        private readonly IApplicationDialog dialog;

        public NotificationValue<bool> Extend { get; } = new();

        public Function2ViewModel(
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

        protected override Task OnNotifyFunction2Async()
        {
            Extend.Value = !Extend.Value;
            return Task.CompletedTask;
        }

        protected override async Task OnNotifyFunction3Async()
        {
            await dialog.Information(Extend.Value ? "Normal-3" : "Extend-3");
        }

        protected override async Task OnNotifyFunction4Async()
        {
            await dialog.Information(Extend.Value ? "Normal-4" : "Extend-4");
        }
    }
}
