namespace WorkKey.FormsApp
{
    using Smart.Navigation;

    using WorkKey.FormsApp.Shell;

    public partial class MainPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        protected override bool OnBackButtonPressed()
        {
            (BindingContext as MainPageViewModel)?.Navigator.NotifyAsync(ShellEvent.Back);
            return true;
        }
    }
}
