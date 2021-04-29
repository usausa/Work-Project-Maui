[assembly: Xamarin.Forms.ExportEffect(typeof(KeySample.FormsApp.Droid.Effects.NoBorderPlatformEffect), nameof(KeySample.FormsApp.Effects.NoBorderEffect))]

namespace KeySample.FormsApp.Droid.Effects
{
    using Android.Widget;

    using Xamarin.Forms.Platform.Android;

    public sealed class NoBorderPlatformEffect : PlatformEffect
    {
        protected override void OnAttached()
        {
            Control.Background = null;
            Control.SetBackgroundColor(Android.Graphics.Color.Transparent);
        }

        protected override void OnDetached()
        {
        }
    }
}
