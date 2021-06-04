using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

using WorkPopupLevel.Droid;

using Xamarin.Forms;

[assembly: ExportRenderer(typeof(Xamarin.CommunityToolkit.UI.Views.Popup<>), typeof(PopupRendererEx))]

namespace WorkPopupLevel.Droid
{
    public class PopupRendererEx : Xamarin.CommunityToolkit.UI.Views.PopupRenderer
    {
        public PopupRendererEx(Context context)
            : base(context)
        {
        }

        public override bool DispatchKeyEvent(KeyEvent e)
        {
            System.Diagnostics.Debug.WriteLine($"**** {e.Action} {e.KeyCode}");
            return base.DispatchKeyEvent(e);
        }
    }
}