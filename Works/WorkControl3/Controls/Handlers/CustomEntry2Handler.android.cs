using System.Diagnostics;
using Android.Views;
using View = Android.Views.View;

namespace WorkControl3.Controls.Handlers;

using AndroidX.AppCompat.Widget;

using Microsoft.Maui.Handlers;

public partial class CustomEntry2Handler : EntryHandler
{
    protected override void ConnectHandler(AppCompatEditText platformView)
    {
        base.ConnectHandler(platformView);

        platformView.SetSelectAllOnFocus(true);
        platformView.ShowSoftInputOnFocus = false;
        platformView.SetOnKeyListener(new CustomOnKeyListener((CustomEntry2)VirtualView));
    }

    protected override void DisconnectHandler(AppCompatEditText platformView)
    {
        platformView.Dispose();

        base.DisconnectHandler(platformView);
    }

    private sealed class CustomOnKeyListener : Java.Lang.Object, View.IOnKeyListener
    {
        private readonly CustomEntry2 entry;

        public CustomOnKeyListener(CustomEntry2 entry)
        {
            this.entry = entry;
        }

        public bool OnKey(View v, Keycode keyCode, KeyEvent e)
        {
            if (e.Action == KeyEventActions.Down && keyCode == Keycode.Enter)
            {
                Debug.WriteLine("* Handled");
                return true;
            }

            return false;
        }
    }
}
