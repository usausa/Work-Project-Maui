[assembly: Xamarin.Forms.ExportEffect(typeof(KeySample.FormsApp.Droid.Effects.SelectAllOnFocusPlatformEffect), nameof(KeySample.FormsApp.Effects.SelectAllOnFocusEffect))]

namespace KeySample.FormsApp.Droid.Effects
{
    using Android.Widget;

    using Xamarin.Forms.Platform.Android;

    public sealed class SelectAllOnFocusPlatformEffect : PlatformEffect
    {
        protected override void OnAttached()
        {
            if (Control is TextView textView)
            {
                textView.SetSelectAllOnFocus(true);
            }
        }

        protected override void OnDetached()
        {
        }
    }
}
