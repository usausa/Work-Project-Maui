using Microsoft.Extensions.DependencyInjection;

namespace WorkSafeArea
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell());
            //return new Window(new MainPage());
        }
    }
}