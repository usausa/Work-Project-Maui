using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace WorkEntry
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private void Entry_OnCompleted(object sender, EventArgs e)
        {
            Debug.WriteLine("**** Entry_OnCompleted");
        }
    }
}
