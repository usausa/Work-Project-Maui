using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AnimationTest
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class View2 : ContentView
    {
        public View2()
        {
            InitializeComponent();
        }

        private async void Button_OnClicked(object sender, EventArgs e)
        {
            await Application.Current.MainPage.DisplayAlert("", "clicked", "ok");
        }
    }
}