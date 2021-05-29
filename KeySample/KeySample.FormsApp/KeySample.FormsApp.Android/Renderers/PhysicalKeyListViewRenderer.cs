[assembly: Xamarin.Forms.ExportRenderer(typeof(Xamarin.Forms.ListView), typeof(KeySample.FormsApp.Droid.Renderers.PhysicalKeyListViewRenderer))]

namespace KeySample.FormsApp.Droid.Renderers
{
    using Android.Content;

    using Xamarin.Forms;
    using Xamarin.Forms.Platform.Android;

    public class PhysicalKeyListViewRenderer : Xamarin.Forms.Platform.Android.ListViewRenderer
    {
        public PhysicalKeyListViewRenderer(Context context)
            : base(context)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<ListView> e)
        {
            base.OnElementChanged(e);
        }
    }
}
