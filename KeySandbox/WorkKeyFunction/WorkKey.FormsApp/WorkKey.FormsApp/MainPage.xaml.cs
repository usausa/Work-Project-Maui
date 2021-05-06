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
            if ((BindingContext is MainPageViewModel vm) && !vm.BusyState.IsBusy)
            {
                vm.Navigator.NotifyAsync(ShellEvent.Back);
            }

            return true;
        }
    }
}
