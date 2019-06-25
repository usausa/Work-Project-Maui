using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace AnimationTest
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        private readonly List<View> views = new List<View>();

        public MainPage()
        {
            InitializeComponent();
        }

        private void Button1_OnClicked(object sender, EventArgs e)
        {
            ClearViews();
            // TODO Animation
            AddView(new BoxView { BackgroundColor = Color.OrangeRed });
            AddView(new BoxView { BackgroundColor = Color.CornflowerBlue });
        }

        private void Button2_OnClicked(object sender, EventArgs e)
        {
            ClearViews();
        }

        private void Button3_OnClicked(object sender, EventArgs e)
        {
            ClearViews();
        }

        private void Button4_OnClicked(object sender, EventArgs e)
        {
            ClearViews();
        }

        // TODO Save and Restore focus after animation
        // TODO Enable ?, IsVisible(stack) ?

        private void AddView(View view)
        {
            AbsoluteLayout.SetLayoutFlags(view, AbsoluteLayoutFlags.WidthProportional | AbsoluteLayoutFlags.HeightProportional);
            AbsoluteLayout.SetLayoutBounds(view, new Rectangle(0, 0, 1, 1));
            // TODO Insert first or last ?
            Container.Children.Add(view);

            views.Add(view);
        }

        private void ClearViews()
        {
            foreach (var view in views)
            {
                Container.Children.Remove(view);
            }

            views.Clear();
        }
    }
}
