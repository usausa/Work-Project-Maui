namespace WorkKey.FormsApp
{
    using Smart.ComponentModel;
    using Smart.Forms.ViewModels;
    using Smart.Navigation;

    using WorkKey.FormsApp.Shell;

    public class MainPageViewModel : ViewModelBase, IShellControl
    {
        public NotificationValue<string> Title { get; } = new();

        public ApplicationState ApplicationState { get; }

        public INavigator Navigator { get; }

        //--------------------------------------------------------------------------------
        // Constructor
        //--------------------------------------------------------------------------------

        public MainPageViewModel(
            ApplicationState applicationState,
            INavigator navigator)
            : base(applicationState)
        {
            ApplicationState = applicationState;
            Navigator = navigator;
        }
    }
}
