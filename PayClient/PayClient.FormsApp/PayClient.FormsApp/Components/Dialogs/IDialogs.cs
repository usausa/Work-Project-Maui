namespace PayClient.FormsApp.Components.Dialogs
{
    using System;
    using System.Threading.Tasks;

    public interface IDialogs
    {
        Task<bool> Confirm(string message, string title = null, string acceptButton = "OK", string cancelButton = "Cancel");

        Task Information(string message, string title = null, string cancelButton = "OK");

        Task<string> Select(string[] items, string title = null);

        IProgress Progress(string title = null);

        IProgress Loading(string title = null);
    }
}
