[assembly: Xamarin.Forms.ExportRenderer(typeof(KeyboardSample.Controls.NoKeyboardEntry), typeof(KeyboardSample.Droid.Renderers.NoKeyboardEntryRenderer))]

namespace KeyboardSample.Droid.Renderers
{
    using System.ComponentModel;

    using Android.Content;

    using KeyboardSample.Droid.Helpers;

    using Xamarin.Forms;
    using Xamarin.Forms.Platform.Android;

    public sealed class NoKeyboardEntryRenderer : EntryRenderer
    {
        public NoKeyboardEntryRenderer(Context context)
            : base(context)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);

            EditText.ShowSoftInputOnFocus = false;

            // Hide border
            if (Control != null)
            {
                Control.Background = null;
                Control.SetBackgroundColor(Android.Graphics.Color.Transparent);
            }
        }

        protected override void OnFocusChangeRequested(object sender, VisualElement.FocusRequestArgs e)
        {
            if (Control == null)
            {
                return;
            }

            e.Result = true;

            if (e.Focus)
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    if ((Control == null) || Control.IsDisposed())
                    {
                        return;
                    }

                    Control.RequestFocus();
                });
            }
            else
            {
                Control.ClearFocus();
            }
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if ((e.PropertyName == VisualElement.IsFocusedProperty.PropertyName) && EditText.IsFocused)
            {
                EditText.HideKeyboard();
            }
        }
    }
}
