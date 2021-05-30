[assembly: Xamarin.Forms.ExportRenderer(typeof(Xamarin.Forms.Entry), typeof(KeySample.FormsApp.Droid.Renderers.PhysicalKeyEntryRenderer))]

namespace KeySample.FormsApp.Droid.Renderers
{
    using System.ComponentModel;

    using Android.Content;
    using Android.Views;
    using Android.Views.InputMethods;
    using Android.Widget;

    using KeySample.FormsApp.Droid.Helpers;

    using Xamarin.Forms;
    using Xamarin.Forms.Platform.Android;

    public class PhysicalKeyEntryRenderer : Xamarin.Forms.Platform.Android.EntryRenderer, TextView.IOnEditorActionListener
    {
        public PhysicalKeyEntryRenderer(Context context)
            : base(context)
        {
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                EditText?.SetOnEditorActionListener(null);
            }

            base.Dispose(disposing);
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);

            // Disable show input on focus
            EditText.ShowSoftInputOnFocus = false;

            // Select all
            EditText.SetSelectAllOnFocus(true);

            // Override editor action
            if (e.OldElement is not null)
            {
                EditText.SetOnEditorActionListener(null);
            }

            if (e.NewElement is not null)
            {
                EditText.SetOnEditorActionListener(this);
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

        bool TextView.IOnEditorActionListener.OnEditorAction(TextView? v, ImeAction actionId, KeyEvent? e)
        {
            if ((e!.KeyCode == Keycode.Enter) && (e.Action == KeyEventActions.Up))
            {
                ((IEntryController)Element).SendCompleted();
            }

            return true;
        }
    }
}
