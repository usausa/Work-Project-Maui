namespace DataMap.FormsApp.Modules
{
    using DataMap.FormsApp.Components.Dialogs;

    using Smart.Forms.Input;

    public class MenuViewModel : AppViewModelBase
    {
        public AsyncCommand TestCommand { get; }

        public MenuViewModel(
            ApplicationState applicationState,
            IDialogs dialogs)
            : base(applicationState)
        {
            TestCommand = MakeAsyncCommand(() => dialogs.Information("Test"));
        }
    }
}
