using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BindingTest
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class View1 : ContentView
	{
		public View1 ()
		{
			InitializeComponent ();
		}

	    private void Button_OnClicked(object sender, EventArgs e)
	    {
            ViewProperty.SetTitle(this, "View1*");
	    }
	}
}