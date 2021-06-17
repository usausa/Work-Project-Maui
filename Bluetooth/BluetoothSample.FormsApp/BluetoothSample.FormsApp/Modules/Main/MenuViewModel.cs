namespace BluetoothSample.FormsApp.Modules.Main
{
    using System.Windows.Input;

    using BluetoothSample.FormsApp.Components.Dialog;
    using BluetoothSample.FormsApp.Components.Meter;

    using Smart.Navigation;

    public class MenuViewModel : AppViewModelBase
    {
        public ICommand DiscoveryCommand { get; }

        public MenuViewModel(
            ApplicationState applicationState,
            IApplicationDialog dialog,
            IMeterReader meterReader)
            : base(applicationState)
        {
            DiscoveryCommand = MakeAsyncCommand(async () =>
            {
                using (dialog.Loading("reading"))
                {
                    await meterReader.ReadAsync();
                }
            });
        }
    }
}
