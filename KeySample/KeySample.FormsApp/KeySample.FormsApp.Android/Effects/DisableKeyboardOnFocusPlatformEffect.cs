[assembly: Xamarin.Forms.ExportEffect(typeof(KeySample.FormsApp.Droid.Effects.DisableKeyboardOnFocusPlatformEffect), nameof(KeySample.FormsApp.Effects.DisableKeyboardOnFocusEffect))]

namespace KeySample.FormsApp.Droid.Effects
{
    using Android.Widget;

    using Xamarin.Forms.Platform.Android;

    public sealed class DisableKeyboardOnFocusPlatformEffect : PlatformEffect
    {
        protected override void OnAttached()
        {
            if (Control is TextView textView)
            {
                textView.ShowSoftInputOnFocus = false;
            }
        }

        protected override void OnDetached()
        {
        }
    }
}
