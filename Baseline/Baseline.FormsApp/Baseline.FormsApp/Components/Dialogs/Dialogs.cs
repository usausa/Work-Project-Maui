namespace Baseline.FormsApp.Components.Dialogs
{
    using System;
    using System.Threading.Tasks;

    using Acr.UserDialogs;

    using Xamarin.Forms;

    public sealed class Dialogs : IDialogs
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

        public async Task<bool> Confirm(string message, string title, string acceptButton, string cancelButton)
        {
            return await Application.Current.MainPage.DisplayAlert(title, message, acceptButton, cancelButton);
        }

        public async Task Information(string message, string title, string cancelButton)
        {
            await Application.Current.MainPage.DisplayAlert(title, message, cancelButton);
        }

        public async Task<string> Select(string[] items, string title)
        {
            return await Application.Current.MainPage.DisplayActionSheet(title, null, null, items);
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
