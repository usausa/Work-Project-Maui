using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Views;
using Android.Widget;

[assembly: Xamarin.Forms.ExportEffect(typeof(WorkList.Droid.ListViewPlatformEffect), nameof(WorkList.ListViewEffect))]

namespace WorkList.Droid
{
    using System;
    using System.Collections.Generic;

    using Xamarin.Forms.Platform.Android;

    public class ListViewPlatformEffect : PlatformEffect
    {
        protected override void OnAttached()
        {
            var listView = (Android.Widget.ListView)Control;
            listView.SetFocusable(ViewFocusability.Focusable);

            listView.SetDrawSelectorOnTop(true);
            var states = new StateListDrawable();
            states.AddState(new[] { Android.Resource.Attribute.StateFocused }, new PaintDrawable(Color.Gray) { Alpha = 64 });
            listView.Selector = states;
        }

        protected override void OnDetached()
        {
        }
    }
}