namespace Template.MobileApp.Modules.Navigation;

public class DialogMenuViewModel : AppViewModelBase
{
    private int count;

    public ICommand InformationCommand { get; }
    public ICommand ConfirmCommand { get; }
    public ICommand Confirm3Command { get; }
    public ICommand SelectCommand { get; }
    public ICommand InputCommand { get; }
    public ICommand IndicatorCommand { get; }
    public ICommand LockCommand { get; }
    public ICommand LoadingCommand { get; }
    public ICommand ProgressCommand { get; }
    public ICommand SnackbarCommand { get; }
    public ICommand ToastCommand { get; }

    public DialogMenuViewModel(
        ApplicationState applicationState,
        IDialog dialog)
        : base(applicationState)
    {
        InformationCommand = MakeAsyncCommand(async () => await dialog.InformationAsync("Information"));
        ConfirmCommand = MakeAsyncCommand(async () =>
        {
            var result = await dialog.ConfirmAsync("Confirm");
            await dialog.InformationAsync($"Result={result}");
        });
        Confirm3Command = MakeAsyncCommand(async () =>
        {
            var result = await dialog.Confirm3Async("Confirm3");
            await dialog.InformationAsync($"Result={result}");
        });
        SelectCommand = MakeAsyncCommand(async () =>
        {
            var result = await dialog.SelectAsync(["Item-1", "Item-2", "Item-3"], cancel: "Cancel");
            await dialog.InformationAsync($"Result={result}");
        });
        InputCommand = MakeAsyncCommand(async () =>
        {
            var result = await dialog.PromptAsync(defaultValue: "123", parameter: new PromptParameter { PromptType = PromptType.Number, MaxLength = 5 });
            if (result.Accepted)
            {
                await dialog.InformationAsync($"Result={result.Text}");
            }
        });
        IndicatorCommand = MakeAsyncCommand(async () =>
        {
            using var loading = dialog.Indicator();

            await Task.Delay(3000);
        });
        LockCommand = MakeAsyncCommand(async () =>
        {
            using var loading = dialog.Lock();

            await Task.Delay(3000);
        });
        LoadingCommand = MakeAsyncCommand(async () =>
        {
            using var loading = dialog.Loading();

            loading.Update("Connecting...");
            await Task.Delay(1000);
            loading.Update("Downloading...");
            await Task.Delay(2000);
            loading.Update("Updating...");
            await Task.Delay(1000);
        });
        ProgressCommand = MakeAsyncCommand(async () =>
        {
            using var loading = dialog.Progress();

            for (var i = 0; i <= 1000; i += 2)
            {
                loading.Update(i / 10d);
                await Task.Delay(1);
            }
        });
        SnackbarCommand = MakeDelegateCommand(() => dialog.Snackbar("Warning", 3000, Colors.Orange));
        ToastCommand = MakeAsyncCommand(async () =>
        {
            count++;
            await dialog.Toast($"Count={count}");
        });
    }

    protected override Task OnNotifyBackAsync() => Navigator.ForwardAsync(ViewId.Menu);

    protected override Task OnNotifyFunction1() => OnNotifyBackAsync();
}
