using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace FocusTest
{
    public partial class App : Application
    {
        public event EventHandler Trace;

        public App()
        {
            InitializeComponent();

            MainPage = new MainPage();
        }

        public void RaiseTrace()
        {
            Trace?.Invoke(this, EventArgs.Empty);
            ;
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
