namespace Business.FormsApp.Components.Dialogs
{
    using System;
    using System.Threading.Tasks;

    public interface IDialogs
    {
        IProgress Progress(string title = null);

        IProgress Loading(string title = null);

        Task<DateDialogResult> Date(string title = null, DateTime? value = null, DateTime? minDate = null, DateTime? maxDate = null);

        Task<TimeDialogResult> Time(string title = null, TimeSpan? value = null);
    }
}
