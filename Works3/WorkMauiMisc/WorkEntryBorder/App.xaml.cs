using Microsoft.Extensions.DependencyInjection;

namespace WorkEntryBorder
{
    public partial class App : Application
    {
        public App()
        {
            //Current!.UserAppTheme = AppTheme.Light;

            InitializeComponent();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new MainPage());
        }
    }
}