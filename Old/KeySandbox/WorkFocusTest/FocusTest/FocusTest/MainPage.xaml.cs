using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace FocusTest
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            DescendantAdded += (sender, args) =>
            {
                if (args.Element is VisualElement visual)
                {
                    visual.Focused += OnFocused;
                    visual.Unfocused += OnUnfocused;
                }
            };

            ((App)Application.Current).Trace += OnTrace;

            InitializeComponent();
        }

        private void OnFocused(object sender, FocusEventArgs e)
        {
            Debug.WriteLine($"*Focused {e.VisualElement.ClassId}");
        }

        private void OnUnfocused(object sender, FocusEventArgs e)
        {
            Debug.WriteLine($"*Unfocused {e.VisualElement.ClassId}");
        }

        private void OnTrace(object sender, EventArgs e)
        {
            Debug.WriteLine("----------");
            var watch = Stopwatch.StartNew();
            Trace(Content);
            Debug.WriteLine(watch.ElapsedMilliseconds);
            Debug.WriteLine("----------");
        }

        private static void TraceOutput(Element element, int index)
        {
            //var indent = new string(' ', Debug.IndentLevel * 2);
            //if (element is VisualElement visual)
            //{
            //    Debug.WriteLine($"{indent}{visual.GetType()} : Index={index} IsFocused={visual.IsFocused} IsVisible={visual.IsVisible} IsEnabled={visual.IsEnabled}");
            //}
            //else
            //{
            //    Debug.WriteLine($"{indent}{element.GetType()} : Index={index}");
            //}
        }

        public void Trace(Element element)
        {
            if ((element is VisualElement visual) &&
                (!visual.IsVisible || !visual.IsEnabled))
            {
                return;
            }

            var index = Tab.GetIndex(element);
            TraceOutput(element, index);
            Debug.Indent();

            if (element is ContentView contentView)
            {
                Trace(contentView.Content);
            }
            else if (element is Layout layout)
            {
                foreach (var child in layout.Children)
                {
                    Trace(child);
                }
            }

            Debug.Unindent();
        }
    }
}
