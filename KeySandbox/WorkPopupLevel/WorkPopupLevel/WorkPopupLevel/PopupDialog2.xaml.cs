using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace WorkPopupLevel
{
    public partial class PopupDialog2
    {
        public PopupDialog2()
        {
            InitializeComponent();
        }

        private void TestButton_OnClicked(object sender, EventArgs e)
        {
            Debug.WriteLine("**Modal : " + Navigation.ModalStack.Count);
            Debug.WriteLine("**Navigation : " + Navigation.NavigationStack.Count);
        }

        private void CloseButton_OnClicked(object sender, EventArgs e)
        {
            Dismiss("ok");
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
        }
    }
}