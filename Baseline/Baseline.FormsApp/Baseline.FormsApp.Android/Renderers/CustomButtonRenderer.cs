[assembly: Xamarin.Forms.ExportRenderer(typeof(Xamarin.Forms.Button), typeof(Baseline.FormsApp.Droid.Renderers.CustomButtonRenderer))]

namespace Baseline.FormsApp.Droid.Renderers
{
    using System;

    using Android.Content;
    using Android.Graphics;

    using Xamarin.Forms;
    using Xamarin.Forms.Platform.Android;

    public sealed class CustomButtonRenderer : ButtonRenderer
    {
        public CustomButtonRenderer(Context context)
            : base(context)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Button> e)
        {
            base.OnElementChanged(e);

            Control.SetAllCaps(false);

            var fontFamily = e.NewElement.FontFamily;
            if ((fontFamily != null) && (fontFamily.EndsWith(".otf", StringComparison.OrdinalIgnoreCase) || fontFamily.EndsWith(".ttf", StringComparison.OrdinalIgnoreCase)))
            {
                Control.Typeface = Typeface.CreateFromAsset(Context.Assets, e.NewElement.FontFamily);
            }
        }
    }
}
