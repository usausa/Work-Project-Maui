[assembly: Xamarin.Forms.ExportRenderer(typeof(Xamarin.Forms.Label), typeof(Baseline.FormsApp.Droid.Renderers.CustomFontLabelRenderer))]

namespace Baseline.FormsApp.Droid.Renderers
{
    using System;

    using Android.Content;
    using Android.Graphics;

    using Xamarin.Forms;
    using Xamarin.Forms.Platform.Android;

    public sealed class CustomFontLabelRenderer : LabelRenderer
    {
        public CustomFontLabelRenderer(Context context)
            : base(context)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Label> e)
        {
            base.OnElementChanged(e);

            var fontFamily = e.NewElement.FontFamily;
            if ((fontFamily != null) && (fontFamily.EndsWith(".otf", StringComparison.OrdinalIgnoreCase) || fontFamily.EndsWith(".ttf", StringComparison.OrdinalIgnoreCase)))
            {
                Control.Typeface = Typeface.CreateFromAsset(Context.Assets, e.NewElement.FontFamily);
            }
        }
    }
}