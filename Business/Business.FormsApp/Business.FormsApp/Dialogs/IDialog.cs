namespace Business.FormsApp.Dialogs
{
    using System;
    using System.Threading.Tasks;

    using Acr.UserDialogs;

    public interface IProgress : IDisposable
    {
        void Update(int percent);
    }

    public class DateDialogResult
    {
        public bool Ok { get; }

        public DateTime Value { get; }

        public DateDialogResult(bool ok, DateTime value)
        {
            Ok = ok;
            Value = value;
        }
    }

    public class TimeDialogResult
    {
        public bool Ok { get; }

        public TimeSpan Value { get; }

        public TimeDialogResult(bool ok, TimeSpan value)
        {
            Ok = ok;
            Value = value;
        }
    }

    public interface IDialog
    {
        IProgress Progress(string title = null);

        IProgress Loading(string title = null);

        Task<DateDialogResult> Date(string title = null, DateTime? value = null, DateTime? minDate = null, DateTime? maxDate = null);

        Task<TimeDialogResult> Time(string title = null, TimeSpan? value = null);
    }

    public sealed class Dialog : IDialog
    {
        private sealed class ProgressWrapper : IProgress
        {
            private readonly IProgressDialog dialog;

            public ProgressWrapper(IProgressDialog dialog)
            {
                this.dialog = dialog;
            }

            public void Dispose()
            {
                dialog.Dispose();
            }

            public void Update(int percent)
            {
                dialog.PercentComplete = percent;
            }
        }

        public IProgress Progress(string title = null)
        {
            return new ProgressWrapper(UserDialogs.Instance.Progress(title));
        }

        public IProgress Loading(string title = null)
        {
            return new ProgressWrapper(UserDialogs.Instance.Loading(title));
        }

        public async Task<DateDialogResult> Date(string title = null, DateTime? value = null, DateTime? minDate = null, DateTime? maxDate = null)
        {
            var result = await UserDialogs.Instance.DatePromptAsync(new DatePromptConfig
            {
                Title = title,
                SelectedDate = value,
                MaximumDate = maxDate,
                MinimumDate = minDate
            });

            return new DateDialogResult(result.Ok, result.SelectedDate);
        }

        public async Task<TimeDialogResult> Time(string title = null, TimeSpan? value = null)
        {
            var result = await UserDialogs.Instance.TimePromptAsync(new TimePromptConfig
            {
                Title = title,
                SelectedTime = value
            });

            return new TimeDialogResult(result.Ok, result.SelectedTime);
        }
    }
}
