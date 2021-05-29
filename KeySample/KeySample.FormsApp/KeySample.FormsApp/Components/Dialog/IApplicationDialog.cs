namespace KeySample.FormsApp.Components.Dialog
{
    using System.Threading.Tasks;

    using XamarinFormsComponents.Dialogs;

    public interface IApplicationDialog
    {
        ValueTask<bool> Confirm(string title, string message, string ok, string cancel);

        ValueTask Information(string title, string message, string ok);

        ValueTask<int> Select(int selected, string[] items);

        IProgress Progress(string title);

        IProgress Loading(string title);
    }
}
