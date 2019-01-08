namespace Business.FormsApp.Modules
{
    using Business.FormsApp.Components.Log;

    using Smart.Forms.Input;
    using Smart.Navigation;

    public class MenuViewModel : AppViewModelBase
    {
        public AsyncCommand<ViewId> ForwardCommand { get; }

        public MenuViewModel(
            ApplicationState applicationState,
            ILogger logger)
            : base(applicationState)
        {
            ForwardCommand = MakeAsyncCommand<ViewId>(x => Navigator.ForwardAsync(x));

            logger.Info("MainViewModel created.");
        }
    }
}
