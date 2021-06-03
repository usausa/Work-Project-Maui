using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.OS;
using Android.Views;
using Android.Widget;

namespace WorkList.Droid
{
    [Activity(Label = "WorkList", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize )]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        private readonly KeyManager keyManager = new KeyManager();

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            LoadApplication(new App(keyManager));
        }

        public override bool DispatchKeyEvent(KeyEvent e)
        {
            if (e.KeyCode == Keycode.DpadUp)
            {
                // MEMO 1 is header
                if ((CurrentFocus is ListView listView) && (listView.SelectedItemPosition > 1))
                {
                }
                else
                {
                    if (e.Action == KeyEventActions.Down)
                    {
                        keyManager.RaiseForward(false);
                    }

                    return true;
                }
            }

            if (e.KeyCode == Keycode.DpadDown)
            {
                // MEMO 2 is header and footer
                if ((CurrentFocus is ListView listView) && (listView.SelectedItemPosition < listView.Adapter.Count - 2))
                {
                }
                else
                {
                    if (e.Action == KeyEventActions.Down)
                    {
                        keyManager.RaiseForward(true);
                    }

                    return true;
                }
            }

            if (e.KeyCode == Keycode.DpadLeft)
            {
                if (CurrentFocus is EditText editText)
                {
                    if ((editText.SelectionStart == 0) && (editText.SelectionEnd == 0))
                    {
                        return true;
                    }
                }
                else
                {
                    return true;
                }
            }
            if (e.KeyCode == Keycode.DpadRight)
            {
                if (CurrentFocus is EditText editText)
                {
                    var textLength = editText.Text?.Length ?? 0;
                    if ((editText.SelectionStart == textLength) && (editText.SelectionEnd == textLength))
                    {
                        return true;
                    }
                }
                else
                {
                    return true;
                }
            }

            return base.DispatchKeyEvent(e);
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}