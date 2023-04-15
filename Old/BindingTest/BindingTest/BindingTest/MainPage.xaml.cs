namespace BindingTest
{
    using System;
    using Xamarin.Forms;

    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private void Button1_OnClicked(object sender, EventArgs e)
        {
            Container.Content = new View1();
        }

        private void Button2_OnClicked(object sender, EventArgs e)
        {
            Container.Content = new View2();
        }

        private void Button3_OnClicked(object sender, EventArgs e)
        {
            GC.Collect();
        }
    }
}
