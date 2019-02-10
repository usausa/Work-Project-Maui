namespace PayClient.FormsApp.Modules
{
    using System.Threading.Tasks;

    using Smart.Forms.Input;
    using Smart.Navigation;

    public class MenuViewModel : AppViewModelBase
    {
        public AsyncCommand PayCommand { get; }

        public AsyncCommand SettingCommand { get; }

        public MenuViewModel(ApplicationState applicationState)
            : base(applicationState)
        {
            PayCommand = MakeAsyncCommand(Pay);
            SettingCommand = MakeAsyncCommand(Setting);
        }

        private Task Pay()
        {
            return Task.CompletedTask;
        }

        private Task Setting()
        {
            return Task.CompletedTask;
        }
    }
}
