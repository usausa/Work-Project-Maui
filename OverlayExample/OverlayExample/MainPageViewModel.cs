namespace OverlayExample;

using System.Windows.Input;

using Smart.Maui.ViewModels;

public class MainPageViewModel : ViewModelBase
{
    public ICommand LockCommand { get; }
    public ICommand LoadingCommand { get; }
    public ICommand ProgressCommand { get; }

    public MainPageViewModel(IDialog dialog)
    {
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

            for (var i = 0; i <= 1000; i++)
            {
                loading.Update(i / 10d);
                await Task.Delay(1);
            }
        });
    }
}
