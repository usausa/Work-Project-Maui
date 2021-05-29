namespace KeySample.FormsApp.Droid.Components.Dialog
{
    using System.Diagnostics.CodeAnalysis;
    using System.Threading.Tasks;

    using Android.App;
    using Android.Content;
    using Android.Graphics.Drawables;
    using Android.Views;

    using KeySample.FormsApp.Components.Dialog;

    using XamarinFormsComponents.Dialogs;

    public class ApplicationDialog : ApplicationDialogBase
    {
        private readonly Activity activity;

        public ApplicationDialog(Activity activity, IDialogs dialogs)
            : base(dialogs)
        {
            this.activity = activity;
        }

        public async override ValueTask<bool> Confirm(string title, string message, string ok, string cancel)
        {
            var dialog = new ConfirmDialog(activity);
            return await dialog.ShowAsync(title, message, ok, cancel);
        }

        public async override ValueTask Information(string title, string message, string ok)
        {
            var dialog = new InformationDialog(activity);
            await dialog.ShowAsync(title, message, ok);
        }

        public async override ValueTask<int> Select(int selected, string[] items)
        {
            var dialog = new SelectDialog(activity);
            return await dialog.ShowAsync(null, selected, items);
        }

        public class ConfirmDialog : Java.Lang.Object, IDialogInterfaceOnKeyListener
        {
            private readonly TaskCompletionSource<bool> result = new();

            private readonly Activity activity;

            [AllowNull]
            private AlertDialog alertDialog;

            public ConfirmDialog(Activity activity)
            {
                this.activity = activity;
            }

            public Task<bool> ShowAsync(string? title, string message, string ok, string cancel)
            {
                alertDialog = new AlertDialog.Builder(activity)
                    .SetTitle(title)!
                    .SetMessage(message)!
                    .SetOnKeyListener(this)!
                    .SetCancelable(false)!
                    .Create()!;
                alertDialog.SetButton((int)DialogButtonType.Positive, ok, (_, _) => result.TrySetResult(true));
                alertDialog.SetButton((int)DialogButtonType.Negative, cancel, (_, _) => result.TrySetResult(false));

                alertDialog.Show();

                return result.Task;
            }

            public bool OnKey(IDialogInterface? dialog, Keycode keyCode, KeyEvent? e)
            {
                if ((e!.KeyCode == Keycode.Del) && (e.Action == KeyEventActions.Up))
                {
                    dialog!.Dismiss();
                    result.TrySetResult(false);
                    return true;
                }

                return false;
            }
        }

        public class InformationDialog : Java.Lang.Object, IDialogInterfaceOnKeyListener
        {
            private readonly TaskCompletionSource<bool> result = new();

            private readonly Activity activity;

            [AllowNull]
            private AlertDialog alertDialog;

            public InformationDialog(Activity activity)
            {
                this.activity = activity;
            }

            public Task ShowAsync(string? title, string message, string ok)
            {
                alertDialog = new AlertDialog.Builder(activity)
                    .SetTitle(title)!
                    .SetMessage(message)!
                    .SetOnKeyListener(this)!
                    .SetCancelable(false)!
                    .Create()!;
                alertDialog.SetButton((int)DialogButtonType.Positive, ok, (_, _) => result.TrySetResult(true));

                alertDialog.Show();

                return result.Task;
            }

            public bool OnKey(IDialogInterface? dialog, Keycode keyCode, KeyEvent? e)
            {
                if ((e!.KeyCode == Keycode.Del) && (e.Action == KeyEventActions.Up))
                {
                    dialog!.Dismiss();
                    result.TrySetResult(false);
                    return true;
                }

                return false;
            }
        }

        public class SelectDialog : Java.Lang.Object, IDialogInterfaceOnKeyListener
        {
            private readonly TaskCompletionSource<int> result = new();

            private readonly Activity activity;

            [AllowNull]
            private AlertDialog alertDialog;

            public SelectDialog(Activity activity)
            {
                this.activity = activity;
            }

            public Task<int> ShowAsync(string? title, int selected, string[] items)
            {
                alertDialog = new AlertDialog.Builder(activity)
                    .SetTitle(title)!
                    .SetItems(items, (_, args) => result.TrySetResult(args.Which))!
                    .SetOnKeyListener(this)!
                    .SetCancelable(false)!
                    .Create()!;
                alertDialog.ListView!.Selector = new ColorDrawable(Android.Graphics.Color.Blue) { Alpha = 64 };

                alertDialog.Show();

                if (selected >= 0)
                {
                    alertDialog.ListView.SetSelection(selected);
                }

                return result.Task;
            }

            public bool OnKey(IDialogInterface? dialog, Keycode keyCode, KeyEvent? e)
            {
                if ((e!.KeyCode == Keycode.Del) && (e.Action == KeyEventActions.Up))
                {
                    dialog!.Dismiss();
                    result.TrySetResult(-1);
                    return true;
                }

                return false;
            }
        }
    }
}
