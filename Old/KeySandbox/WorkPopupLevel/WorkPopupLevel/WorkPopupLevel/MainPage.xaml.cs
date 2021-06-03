using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Rg.Plugins.Popup.Events;
using Rg.Plugins.Popup.Services;
using Xamarin.CommunityToolkit.Extensions;
using Xamarin.Forms;

using XamarinFormsComponents.Dialogs;

namespace WorkPopupLevel
{
    public partial class MainPage : ContentPage
    {
        private XamarinFormsComponents.Dialogs.Dialogs dialogs;

        public MainPage()
        {
            InitializeComponent();

            dialogs = new Dialogs();

            PopupNavigation.Instance.Popping += InstanceOnPopping;
            PopupNavigation.Instance.Pushed += InstanceOnPushed;
        }

        private void InstanceOnPopping(object sender, PopupNavigationEventArgs e)
        {
        }

        private void InstanceOnPushed(object sender, PopupNavigationEventArgs e)
        {
        }

        private async void Popup1Button_OnClicked(object sender, EventArgs e)
        {
            await PopupNavigation.Instance.PushAsync(new PopupDialog1() { AndroidTalkbackAccessibilityWorkaround = true });
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

        private async void Chose1Button_OnClicked(object sender, EventArgs e)
        {
            // UserDialog
            await dialogs.Select(Enumerable.Range(1, 10).Select(x => $"Item-{x}"));
        }

        private async void Chose2Button_OnClicked(object sender, EventArgs e)
        {
            var items = Enumerable.Range(1, 40).Select(x => $"Item-{x}").ToArray();

            var selected = await ((App)Application.Current).Dialog.Select(25, items);
            Debug.WriteLine(selected);
        }

        private async void ConfirmButton_OnClicked(object sender, EventArgs e)
        {
            var ret = await ((App) Application.Current).Dialog.Confirm("title", "message", "ok", "cancel");
            Debug.WriteLine(ret);
        }

        private async void InformationButton_OnClicked(object sender, EventArgs e)
        {
            await ((App) Application.Current).Dialog.Information("title", "message", "ok");
        }
    }
}
