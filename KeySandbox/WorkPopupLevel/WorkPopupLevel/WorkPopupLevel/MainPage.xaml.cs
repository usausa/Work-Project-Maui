using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rg.Plugins.Popup.Services;
using Xamarin.CommunityToolkit.Extensions;
using Xamarin.Forms;

namespace WorkPopupLevel
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();

        }

        private async void Popup1Button_OnClicked(object sender, EventArgs e)
        {
            await PopupNavigation.Instance.PushAsync(new PopupDialog1());
        }

        private async void Popup2Button_OnClicked(object sender, EventArgs e)
        {
            var ret = await Navigation.ShowPopupAsync(new PopupDialog2());
            Debug.WriteLine(ret);
        }

        private void TestButton_OnClicked(object sender, EventArgs e)
        {
            Debug.WriteLine("----------");
            var view = Parent;
            while (view != null)
            {
                Debug.WriteLine(view.GetType() + " " + view.GetHashCode());
                view = view.Parent;
            }
            Debug.WriteLine("----------");
        }

        private void MainPage_OnDescendantAdded(object sender, ElementEventArgs e)
        {
            Debug.WriteLine($"===A {e.Element.GetType()}");
        }

        private void MainPage_OnDescendantRemoved(object sender, ElementEventArgs e)
        {
            Debug.WriteLine($"===R {e.Element.GetType()}");
        }
    }
}
