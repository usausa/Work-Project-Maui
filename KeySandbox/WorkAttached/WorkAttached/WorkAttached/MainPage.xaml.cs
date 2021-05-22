using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace WorkAttached
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private void Button_OnClicked(object sender, EventArgs e)
        {
            Container.Children.Clear();
            Container.Children.Add(new View1());
        }

        private void MainPage_OnDescendantAdded(object sender, ElementEventArgs e)
        {
            Debug.WriteLine($"**** DescendantAdded {e.Element.GetType()}");
        }

        private void MainPage_OnDescendantRemoved(object sender, ElementEventArgs e)
        {
            Debug.WriteLine($"**** DescendantRemoved {e.Element.GetType()}");
        }
    }
}
