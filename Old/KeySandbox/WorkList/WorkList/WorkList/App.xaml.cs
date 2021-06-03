using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace WorkList
{
    public partial class App : Application
    {
        public KeyManager KeyManager { get; }

        public App(KeyManager keyManager)
        {
            KeyManager = keyManager;

            InitializeComponent();

            MainPage = new MainPage();
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
