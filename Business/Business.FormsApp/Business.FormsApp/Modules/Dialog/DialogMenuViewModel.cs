namespace Business.FormsApp.Modules.Dialog
{
    using System.Threading.Tasks;

    using Business.FormsApp.Dialogs;

    using Smart.Forms.Components;
    using Smart.Forms.Input;
    using Smart.Navigation;

    public class DialogMenuViewModel : AppViewModelBase
    {
        private readonly IDialogService dialogService;

        private readonly IDialog dialog;

        public AsyncCommand<ViewId> ForwardCommand { get; }

        public AsyncCommand ProgressCommand { get; }
        public AsyncCommand LoadingCommand { get; }
        public AsyncCommand DateCommand { get; }
        public AsyncCommand TimeCommand { get; }

        public DialogMenuViewModel(
            ApplicationState applicationState,
            IDialogService dialogService,
            IDialog dialog)
            : base(applicationState)
        {
            this.dialogService = dialogService;
            this.dialog = dialog;

            ForwardCommand = MakeAsyncCommand<ViewId>(x => Navigator.ForwardAsync(x));
            ProgressCommand = MakeAsyncCommand(Progress);
            LoadingCommand = MakeAsyncCommand(Loading);
            DateCommand = MakeAsyncCommand(Date);
            TimeCommand = MakeAsyncCommand(Time);
        }

        protected override Task OnNotifyBackAsync()
        {
            return Navigator.ForwardAsync(ViewId.Menu);
        }

        private async Task Progress()
        {
            using (var progress = dialog.Progress())
            {
                for (var i = 0; i < 100; i++)
                {
                    await Task.Delay(50);

                    progress.Update(i + 1);
                }
            }
        }

        private async Task Loading()
        {
            using (dialog.Loading())
            {
                await Task.Delay(3000);
            }
        }

        private async Task Date()
        {
            var result = await dialog.Date();
            if (result.Ok)
            {
                await dialogService.DisplayAlert("Result", result.Value.ToString("yyyy/MM/dd"), "ok");
            }
        }

        private async Task Time()
        {
            var result = await dialog.Time();
            if (result.Ok)
            {
                await dialogService.DisplayAlert("Result", result.Value.ToString(@"hh\:mm"), "ok");
            }
        }
    }
}
