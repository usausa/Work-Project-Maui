namespace BindingTest
{
    using System;

    public partial class View2
    {
        public View2()
        {
            InitializeComponent();
        }

        ~View2()
        {
            System.Diagnostics.Debug.WriteLine("~View2");
        }

        private void Button_OnClicked(object sender, EventArgs e)
        {
            ViewProperty.SetTitle(this, "View2*");
        }
    }
}