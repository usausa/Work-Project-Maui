namespace ViewSample.FormsApp.Droid.Components.Dialog
{
    using System.Threading.Tasks;

    using Android.App;
    using Android.Content;

    using ViewSample.FormsApp.Components.Dialog;

    using XamarinFormsComponents.Dialogs;

    public class ApplicationDialog : ApplicationDialogBase
    {
        private readonly Activity activity;

        public ApplicationDialog(Activity activity, IDialogs dialogs)
            : base(dialogs)
        {
            this.activity = activity;
        }

        public override async ValueTask<bool> Confirm(string title, string message, string ok, string cancel)
        {
            var result = new TaskCompletionSource<bool>();

            var alert = new AlertDialog.Builder(activity).Create();
            alert.SetTitle(title);
            alert.SetMessage(message);
            alert.SetButton((int)DialogButtonType.Positive, ok, (_, _) => result.TrySetResult(true));
            alert.SetButton((int)DialogButtonType.Negative, cancel, (_, _) => result.TrySetResult(false));
            alert.SetCancelable(false);
            alert.Show();

            return await result.Task;
        }

        public override async ValueTask Information(string title, string message, string ok)
        {
            var result = new TaskCompletionSource<bool>();

            var alert = new AlertDialog.Builder(activity).Create();
            alert.SetTitle(title);
            alert.SetMessage(message);
            alert.SetButton((int)DialogButtonType.Positive, ok, (_, _) => result.TrySetResult(true));
            alert.SetCancelable(false);
            alert.Show();

            await result.Task;
        }
    }
}
