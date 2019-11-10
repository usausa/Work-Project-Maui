namespace Baseline.FormsApp
{
    using System.ComponentModel;

    using Baseline.FormsApp.Shell;

    using Smart.Navigation;

    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(true)]
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
