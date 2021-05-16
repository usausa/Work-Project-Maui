using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rg.Plugins.Popup.Services;
using Smart.Forms;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace WorkPopupLevel
{
    public partial class PopupDialog1
    {
        public PopupDialog1()
        {
            InitializeComponent();
        }

        private void TestButton_OnClicked(object sender, EventArgs e)
        {
            Page page = null;

            var find1 = Parent.LogicalChildren.IndexOf(this);
            Debug.WriteLine("find " + find1);

            Debug.WriteLine("----------");
            var view = Parent;
            while (view != null)
            {
                if (view is Page)
                {
                    page = (Page)view;
                }

                Debug.WriteLine(view.GetType() + " " + view.GetHashCode());

                if (view.Parent != null)
                {
                    var find = view.Parent.LogicalChildren.IndexOf(view);
                    Debug.WriteLine(" find " + find);
                }


                view = view.Parent;
            }
            Debug.WriteLine("----------");

            if (page != null)
            {
                foreach (var element in FindChildren(page, 0))
                {
                    //Debug.WriteLine(element.GetType() + " " + element.GetHashCode());
                }

                Debug.WriteLine("----------");
            }
        }

        private async void CloseButton_OnClicked(object sender, EventArgs e)
        {
            await PopupNavigation.Instance.PopAsync();
        }

        public static IEnumerable<Element> FindChildren(Element parent, int level)
        {
            var indent = new string(' ', level * 2);

            foreach (var child in parent.LogicalChildren)
            {
                Debug.WriteLine($"**{indent} {child.GetType()} {child.GetHashCode()}");
                yield return child;

                foreach (var child2 in FindChildren(child, level + 1))
                {
                    yield return child2;
                }
            }

            //var indent = new string(' ', level * 2);

            //if (parent is ContentView contentView)
            //{
            //    // ContentView
            //    var value = contentView.Content;
            //    if (value is Element typedValue)
            //    {
            //        Debug.WriteLine($"**{indent} {typedValue.GetType()} {typedValue.GetHashCode()}");
            //        yield return typedValue;
            //    }

            //    foreach (var child in FindChildren(value, level + 1))
            //    {
            //        yield return child;
            //    }
            //}
            //else if (parent is ContentPage contentPage)
            //{
            //    // ContentPage
            //    var value = contentPage.Content;
            //    if (value is Element typedValue)
            //    {
            //        Debug.WriteLine($"**{indent} {typedValue.GetType()} {typedValue.GetHashCode()}");
            //        yield return typedValue;
            //    }

            //    foreach (var child in FindChildren(value, level + 1))
            //    {
            //        yield return child;
            //    }
            //}
            //else if (parent is Layout layout)
            //{
            //    // Layout
            //    foreach (var child in layout.Children)
            //    {
            //        Debug.WriteLine($"**{indent} {child.GetType()} {child.GetHashCode()}");
            //        yield return child;

            //        foreach (var elementChild in FindChildren(child, level + 1))
            //        {
            //            yield return elementChild;
            //        }
            //    }
            //}
        }

        private void PopupDialog1_OnDisappearing(object sender, EventArgs e)
        {
            Debug.WriteLine("*******************PopupDialog1_OnDisappearing");
        }
    }
}