namespace ViewSample
{
    using Smart.ComponentModel;
    using Smart.Forms.ViewModels;
    using Smart.Navigation;

    using ViewSample.Shell;

    public class MainPageViewModel : ViewModelBase, IShellControl
    {
        public static MainPageViewModel DesignInstance { get; } = null; // For design

        public NotificationValue<string> Title { get; } = new NotificationValue<string>();

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
