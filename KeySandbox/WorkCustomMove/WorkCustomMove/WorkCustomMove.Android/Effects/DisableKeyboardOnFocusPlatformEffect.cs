[assembly: Xamarin.Forms.ExportEffect(typeof(WorkCustomMove.Droid.Effects.DisableKeyboardOnFocusPlatformEffect), nameof(WorkCustomMove.Effects.DisableKeyboardOnFocusEffect))]

namespace WorkCustomMove.Droid.Effects
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
