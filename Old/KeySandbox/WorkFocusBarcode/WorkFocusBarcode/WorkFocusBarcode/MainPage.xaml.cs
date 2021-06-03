using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using ZXing;

namespace WorkFocusBarcode
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private void Button_OnClicked(object sender, EventArgs e)
        {
            Entry1.Text = string.Empty;
            Entry2.Text = string.Empty;
            Entry1.Focus();
        }
    }
}
