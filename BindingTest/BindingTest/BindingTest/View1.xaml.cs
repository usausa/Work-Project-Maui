namespace BindingTest
{
    using System;

    public partial class View1
	{
		public View1 ()
		{
			InitializeComponent ();
		}

	    ~View1()
	    {
            System.Diagnostics.Debug.WriteLine("~View1");
	    }

	    private void Button_OnClicked(object sender, EventArgs e)
	    {
            ViewProperty.SetTitle(this, "View1*");
	    }
	}
}