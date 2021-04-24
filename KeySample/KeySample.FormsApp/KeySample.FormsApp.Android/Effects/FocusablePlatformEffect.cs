[assembly: Xamarin.Forms.ExportEffect(typeof(KeySample.FormsApp.Droid.Effects.FocusablePlatformEffect), nameof(KeySample.FormsApp.Effects.FocusableEffect))]

namespace KeySample.FormsApp.Droid.Effects
{
    using Xamarin.Forms.Platform.Android;

    public sealed class FocusablePlatformEffect : PlatformEffect
    {
        protected override void OnAttached()
        {
            Control.FocusableInTouchMode = true;
        }

        protected override void OnDetached()
        {
        }
    }
}
