[assembly: Xamarin.Forms.ExportRenderer(typeof(Xamarin.Forms.Label), typeof(FontTest.Droid.CustomFontLabelRenderer))]

namespace FontTest.Droid
{
    using Android.Content;
    using Android.Graphics;
    using Android.Widget;
    using Xamarin.Forms;
    using Xamarin.Forms.Platform.Android;

    public class CustomFontLabelRenderer : LabelRenderer
    {
        public CustomFontLabelRenderer(Context context)
            : base(context)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Label> e)
        {
            base.OnElementChanged(e);

            var fontFamily = e.NewElement.FontFamily?.ToLower();
            if (fontFamily != null && (fontFamily.EndsWith(".otf") || fontFamily.EndsWith(".ttf")))
            {
                Control.Typeface = Typeface.CreateFromAsset(Context.Assets, e.NewElement.FontFamily);
            }
        }
    }
}