[assembly: Xamarin.Forms.ExportEffect(typeof(WorkCustomMove.Droid.Effects.EntryPlatformEffect), nameof(WorkCustomMove.Effects.EntryEffect))]

namespace WorkCustomMove.Droid.Effects
{
    using Android.Views;
    using Android.Widget;

    using Xamarin.Forms.Platform.Android;

    public sealed class EntryPlatformEffect : PlatformEffect
    {
        protected override void OnAttached()
        {
            if (Control is TextView textView)
            {
                textView.KeyPress += OnKeyPress;
            }
        }

        private void OnKeyPress(object sender, View.KeyEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine($"==== KeyPress {e.Event?.KeyCode}");

            if (e.KeyCode == Keycode.DpadLeft)
            {
                if (Control is EditText editText)
                {
                    if ((editText.SelectionStart > 0) || (editText.SelectionEnd > 0))
                    {
                        e.Handled = false;
                    }
                }
            }
            if (e.KeyCode == Keycode.DpadRight)
            {
                if (Control is EditText editText)
                {
                    var textLength = editText.Text?.Length ?? 0;
                    if ((editText.SelectionStart < textLength) || (editText.SelectionEnd < textLength))
                    {
                        e.Handled = false;
                    }
                }
            }
        }

        protected override void OnDetached()
        {
        }
    }
}