using System;
using System.Diagnostics;
using System.Linq;

using Xamarin.Forms;

namespace WorkLayout
{
    public partial class MainPage
    {
        public MainPage()
        {
            InitializeComponent();

            Container.LayoutChanged += (_, _) => Debug.WriteLine("* Container.LayoutChanged");
            Container.ChildAdded += (_, args) => Debug.WriteLine($"* Container.ChildAdded type=[{args.Element.GetType()}] id=[{args.Element.Id}]");
            Container.ChildRemoved += (_, args) => Debug.WriteLine($"* Container.ChildRemoved type=[{args.Element.GetType()}] id=[{args.Element.Id}]");
            Container.ChildrenReordered += (_, _) => Debug.WriteLine("* Container.ChildrenReordered");
            Container.DescendantAdded += (_, args) => Debug.WriteLine($"* Container.DescendantAdded type=[{args.Element.GetType()}] id=[{args.Element.Id}]");
            Container.DescendantRemoved += (_, args) => Debug.WriteLine($"* Container.DescendantRemoved type=[{args.Element.GetType()}] id=[{args.Element.Id}]");
        }

        private void ButtonForward1_OnClicked(object sender, EventArgs e)
        {
            var view = new StackLayout
            {
                BackgroundColor = Color.LightBlue
            };
            view.Children.Add(new Button { Text = "Default" });

            Forward(view);
        }

        private void ButtonForward2_OnClicked(object sender, EventArgs e)
        {
            var view = new StackLayout
            {
                BackgroundColor = Color.LightCoral
            };
            view.Children.Add(new Button { Text = "Default" });

            Forward(view);
        }

        private void Forward(View view)
        {
            Container.Children.Clear();

            AbsoluteLayout.SetLayoutFlags(view, AbsoluteLayoutFlags.WidthProportional | AbsoluteLayoutFlags.HeightProportional);
            AbsoluteLayout.SetLayoutBounds(view, new Rectangle(0, 0, 1, 1));

            Container.Children.Add(view);
        }

        private void ButtonPush_OnClicked(object sender, EventArgs e)
        {
        }

        private void ButtonPop_OnClicked(object sender, EventArgs e)
        {
        }

        private void ButtonAdd_OnClicked(object sender, EventArgs e)
        {
            var view = FindLastView();
            if (view is not null)
            {
                view.Children.Add(new Button { Text = $"Button-{view.Children.Count}" });
            }
        }

        private void ButtonRemove_OnClicked(object sender, EventArgs e)
        {
            var view = FindLastView();
            if (view is not null)
            {
                var button = view.Children.OfType<Button>().LastOrDefault();
                if (button is not null)
                {
                    view.Children.Remove(button);
                }
            }
        }

        private StackLayout FindLastView()
        {
            return Container.Children.OfType<StackLayout>().LastOrDefault();
        }
    }
}
