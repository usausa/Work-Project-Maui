using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace BindingTest
{
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
    }
}
