namespace DialogExample.Components.Dialog;

using Android.App;
using Android.Content;
using Android.Graphics.Drawables;
using Android.Views;


internal partial class DialogImplementation
{
    public async partial ValueTask Information(string message, string? title, string ok)
    {
        var dialog = new InformationDialog(CurrentActivity.Activity);
        await dialog.ShowAsync(message, title, ok);
    }

    public async partial ValueTask<bool> Confirm(string message, bool defaultPositive, string? title, string ok, string cancel)
    {
        var dialog = new ConfirmDialog(CurrentActivity.Activity);
        return await dialog.ShowAsync(message, defaultPositive, title, ok, cancel);
    }

    public async partial ValueTask<int> Select(string[] items, int selected, string? title)
    {
        var dialog = new SelectDialog(CurrentActivity.Activity);
        return await dialog.ShowAsync(items, selected, title);
    }
}

internal class InformationDialog : Java.Lang.Object, IDialogInterfaceOnShowListener, IDialogInterfaceOnKeyListener
{
    private readonly TaskCompletionSource<bool> result = new();

    private readonly Activity activity;

    private AlertDialog alertDialog = default!;

    public InformationDialog(Activity activity)
    {
        this.activity = activity;
    }

    public Task ShowAsync(string message, string? title, string ok)
    {
        alertDialog = new AlertDialog.Builder(activity)
            .SetTitle(title)!
            .SetMessage(message)!
            .SetOnKeyListener(this)!
            .SetCancelable(false)!
            .Create()!;
        alertDialog.SetOnShowListener(this);
        alertDialog.SetButton((int)DialogButtonType.Positive, ok, (_, _) => result.TrySetResult(true));

        alertDialog.Show();

        return result.Task;
    }

    public void OnShow(IDialogInterface? dialog)
    {
        var button = alertDialog.GetButton((int)DialogButtonType.Positive)!;
        button.RequestFocus();
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

internal class ConfirmDialog : Java.Lang.Object, IDialogInterfaceOnShowListener, IDialogInterfaceOnKeyListener
{
    private readonly TaskCompletionSource<bool> result = new();

    private readonly Activity activity;

    private AlertDialog alertDialog = default!;

    private bool positive;

    public ConfirmDialog(Activity activity)
    {
        this.activity = activity;
    }

    public Task<bool> ShowAsync(string message, bool defaultPositive, string? title, string ok, string cancel)
    {
        positive = defaultPositive;

        alertDialog = new AlertDialog.Builder(activity)
            .SetTitle(title)!
            .SetMessage(message)!
            .SetOnKeyListener(this)!
            .SetCancelable(false)!
            .Create()!;
        alertDialog.SetOnShowListener(this);
        alertDialog.SetButton((int)DialogButtonType.Positive, ok, (_, _) => result.TrySetResult(true));
        alertDialog.SetButton((int)DialogButtonType.Negative, cancel, (_, _) => result.TrySetResult(false));

        alertDialog.Show();

        return result.Task;
    }

    public void OnShow(IDialogInterface? dialog)
    {
        var button = alertDialog.GetButton(positive ? (int)DialogButtonType.Positive : (int)DialogButtonType.Negative)!;
        button.RequestFocus();
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

internal class SelectDialog : Java.Lang.Object, IDialogInterfaceOnShowListener, IDialogInterfaceOnKeyListener
{
    private readonly TaskCompletionSource<int> result = new();

    private readonly Activity activity;

    private AlertDialog alertDialog = default!;

    public SelectDialog(Activity activity)
    {
        this.activity = activity;
    }

    public Task<int> ShowAsync(string[] items, int selected, string? title)
    {
        alertDialog = new AlertDialog.Builder(activity)
            .SetTitle(title)!
            .SetItems(items, (_, args) => result.TrySetResult(args.Which))!
            .SetOnKeyListener(this)!
            .SetCancelable(false)!
            .Create()!;
        alertDialog.SetOnShowListener(this);
        alertDialog.ListView!.Selector = new ColorDrawable(Android.Graphics.Color.OrangeRed) { Alpha = 64 };

        alertDialog.Show();

        if (selected >= 0)
        {
            alertDialog.ListView.SetSelection(selected);
        }

        return result.Task;
    }

    public void OnShow(IDialogInterface? dialog)
    {
        alertDialog.ListView?.RequestFocus();
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
