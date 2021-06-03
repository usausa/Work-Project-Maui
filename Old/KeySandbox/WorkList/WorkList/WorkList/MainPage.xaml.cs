using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace WorkList
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();

            ((App)Application.Current).KeyManager.Forward += KeyManagerOnForward;
        }

        private void KeyManagerOnForward(object sender, ForwardEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("**** KeyManagerOnForward");

            //var elements = EnumerateVisualElement(this).ToList();
            //System.Diagnostics.Debug.WriteLine("--------");
            //foreach (var visualElement in elements)
            //{
            //    System.Diagnostics.Debug.WriteLine($"**** {visualElement.GetType()} {visualElement.IsFocused}");
            //}

            //if (elements.Count > 0)
            //{
            //    var index = -1;
            //    for (var i = 0; i < elements.Count; i++)
            //    {
            //        if (elements[i].IsFocused)
            //        {
            //            index = i;
            //            break;
            //        }
            //    }

            //    if (index == -1)
            //    {
            //        elements[0].Focus();
            //    }
            //    else if (e.Forward)
            //    {
            //        if (index < elements.Count - 1)
            //        {
            //            var ret = elements[index + 1].Focus();
            //            //System.Diagnostics.Debug.WriteLine($"**** focus={ret}");
            //        }
            //    }
            //    else
            //    {
            //        if (index > 0)
            //        {
            //            var ret = elements[index - 1].Focus();
            //            //System.Diagnostics.Debug.WriteLine($"**** focus={ret}");
            //        }
            //    }
            //}

            e.Handled = true;

            var find = false;
            var first = default(VisualElement);
            var previous = default(VisualElement);
            foreach (var visual in EnumerateVisualElement(this))
            {
                if (visual.IsFocused)
                {
                    if (e.Forward)
                    {
                        find = true;
                    }
                    else
                    {
                        previous?.Focus();
                        return;
                    }
                }
                else if (find)
                {
                    visual.Focus();
                    return;
                }

                previous = visual;
                first ??= visual;
            }

            if (!find)
            {
                first?.Focus();
            }
        }

        public static IEnumerable<VisualElement> EnumerateVisualElement(Element parent)
        {
            foreach (var child in parent.LogicalChildren)
            {
                if (child is VisualElement visualElement)
                {
                    if (!visualElement.IsEnabled || !visualElement.IsVisible)
                    {
                        continue;
                    }

                    if (visualElement.IsTabStop && !(visualElement is Layout))
                    {
                        yield return visualElement;
                    }
                }

                foreach (var descendant in EnumerateVisualElement(child))
                {
                    yield return descendant;
                }
            }
        }
    }
}
