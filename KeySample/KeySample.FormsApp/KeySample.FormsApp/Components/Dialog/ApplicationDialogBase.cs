namespace KeySample.FormsApp.Components.Dialog
{
    using System.Threading.Tasks;

    using XamarinFormsComponents.Dialogs;

    public abstract class ApplicationDialogBase : IApplicationDialog
    {
        private readonly IDialogs dialogs;

        protected ApplicationDialogBase(IDialogs dialogs)
        {
            this.dialogs = dialogs;
        }

        public abstract ValueTask<bool> Confirm(string title, string message, string ok, string cancel);

        public abstract ValueTask Information(string title, string message, string ok);

        public IProgress Progress(string title)
        {
            return dialogs.Progress(title);
        }

        public IProgress Loading(string title)
        {
            return dialogs.Loading(title);
        }
    }

    public static class ApplicationDialogBaseExtensions
    {
        public static ValueTask<bool> Confirm(this IApplicationDialog dialog, string title, string message)
        {
            return dialog.Confirm(title, message, "Yes", "No");
        }

        public static ValueTask<bool> Confirm(this IApplicationDialog dialog, string message)
        {
            return dialog.Confirm(string.Empty, message, "Yes", "No");
        }

        public static ValueTask Information(this IApplicationDialog dialog, string title, string message)
        {
            return dialog.Information(title, message, "OK");
        }

        public static ValueTask Information(this IApplicationDialog dialog, string message)
        {
            return dialog.Information(string.Empty, message, "OK");
        }
    }
}
