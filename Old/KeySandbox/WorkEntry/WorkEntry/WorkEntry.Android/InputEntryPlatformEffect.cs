using System.ComponentModel;

using Android.Text;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;

[assembly: Xamarin.Forms.ExportEffect(typeof(WorkEntry.Droid.InputEntryPlatformEffect), nameof(WorkEntry.InputEntryEffect))]

namespace WorkEntry.Droid
{
    using Xamarin.Forms;
    using Xamarin.Forms.Platform.Android;

    public sealed class InputEntryPlatformEffect : PlatformEffect
    {
        private readonly EditorActionListener listener;

        public InputEntryPlatformEffect()
        {
            listener = new EditorActionListener(this);
        }

        protected override void OnAttached()
        {
            if (Control is EditText editText)
            {
                editText.SetOnEditorActionListener(listener);
            }
        }

        protected override void OnDetached()
        {
            if (Control is EditText editText)
            {
                editText.SetOnEditorActionListener(null);
            }
        }

        protected override void OnElementPropertyChanged(PropertyChangedEventArgs args)
        {
            base.OnElementPropertyChanged(args);
        }

        private class EditorActionListener : Java.Lang.Object, TextView.IOnEditorActionListener
        {
            private readonly InputEntryPlatformEffect parent;

            public EditorActionListener(InputEntryPlatformEffect parent)
            {
                this.parent = parent;
            }

            public bool OnEditorAction(TextView? v, ImeAction actionId, KeyEvent? e)
            {
                if ((e.KeyCode == Keycode.Enter) && (e.Action == KeyEventActions.Up))
                {
                    ((IEntryController)parent.Element).SendCompleted();
                }

                return true;
            }
        }
    }
}