using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace WorkCustomMove
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

            var elements = EnumerateVisualElement(this).ToList();
            //System.Diagnostics.Debug.WriteLine("--------");
            //foreach (var visualElement in elements)
            //{
            //    System.Diagnostics.Debug.WriteLine($"**** {Debug.GetId(visualElement)} {visualElement.GetType()} {visualElement.IsFocused}");
            //}

            if (elements.Count > 0)
            {
                var index = -1;
                for (var i = 0; i < elements.Count; i++)
                {
                    if (elements[i].IsFocused)
                    {
                        index = i;
                        break;
                    }
                }

                if (index == -1)
                {
                    elements[0].Focus();
                }
                else if (e.Forward)
                {
                    if (index < elements.Count - 1)
                    {
                        var ret = elements[index + 1].Focus();
                        //System.Diagnostics.Debug.WriteLine($"**** focus={ret}");
                    }
                }
                else
                {
                    if (index > 0)
                    {
                        var ret = elements[index - 1].Focus();
                        //System.Diagnostics.Debug.WriteLine($"**** focus={ret}");
                    }
                }
            }

            e.Handled = true;
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


        //private void FocusNextByTabIndex()
        //{
        //    var current = FindCurrentFocused(this);
        //    if (current != null)
        //    {
        //        var tabs = current.GetTabIndexesOnParentPage(out var count);
        //        var currentIndex = current.TabIndex;
        //        var next = current.FindNextElement(e.Forward, tabs, ref currentIndex);
        //        (next as VisualElement)?.Focus();
        //    }
        //}

        private VisualElement FindCurrentFocused(Element parent)
        {
            foreach (var child in parent.LogicalChildren)
            {
                if (child is VisualElement visualElement)
                {
                    if (visualElement.IsFocused)
                    {
                        return visualElement;
                    }


                    if (!visualElement.IsEnabled || !visualElement.IsVisible)
                    {
                        continue;
                    }
                }

                visualElement = FindCurrentFocused(child);
                if (visualElement != null)
                {
                    return visualElement;
                }
            }

            return null;
        }

        private void Button_OnClicked(object sender, EventArgs e)
        {
            var visualElement = (VisualElement)sender;
            System.Diagnostics.Debug.WriteLine($"**** Button_OnClicked {Debug.GetId(visualElement)} {visualElement.GetType()} {visualElement.IsFocused}");
        }
    }
}
