namespace PayClient.FormsApp.Modules
{
    using System.Threading.Tasks;

    using PayClient.FormsApp.Components.Barcode;
    using PayClient.FormsApp.Components.Dialogs;

    using Smart.Forms.Input;

    public class MenuViewModel : AppViewModelBase
    {
        private readonly IDialogs dialogs;

        private readonly IBarcodeReader barcodeReader;

        public AsyncCommand PayCommand { get; }

        public AsyncCommand SettingCommand { get; }

        public MenuViewModel(
            ApplicationState applicationState,
            IDialogs dialogs,
            IBarcodeReader barcodeReader)
            : base(applicationState)
        {
            this.dialogs = dialogs;
            this.barcodeReader = barcodeReader;

            PayCommand = MakeAsyncCommand(Pay);
            SettingCommand = MakeAsyncCommand(Setting);
        }

        private async Task Pay()
        {
            if (await Permissions.IsPermissionRequired() &&
                !await Permissions.RequestPermissions())
            {
                return;
            }

            var code = await barcodeReader.Scan();
            await dialogs.Information(code.Text);
        }

        private async Task Setting()
        {
            if (await Permissions.IsPermissionRequired() &&
                !await Permissions.RequestPermissions())
            {
                return;
            }

            var code = await barcodeReader.Scan();
            await dialogs.Information(code.Text);
        }
    }
}
