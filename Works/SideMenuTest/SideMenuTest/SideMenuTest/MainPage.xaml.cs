using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace SideMenuTest
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private void PanGestureRecognizer_OnPanUpdated(object sender, PanUpdatedEventArgs e)
        {
            Debug.WriteLine($"{e.StatusType} {e.TotalX} {e.TotalY}");
        }

        private void ImageButton_OnClicked(object sender, EventArgs e)
        {
            Debug.WriteLine("Clicked");
        }
    }
}
