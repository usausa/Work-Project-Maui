namespace OverlayExample;

using System.Windows.Input;

using Smart.Maui.ViewModels;

public class MainPageViewModel : ViewModelBase
{
    public ICommand SimpleCommand { get; }
    public ICommand TextCommand { get; }

    public MainPageViewModel(IDialog dialog)
    {
        SimpleCommand = MakeAsyncCommand(async () =>
        {
            using var loading = dialog.Loading();

            await Task.Delay(3000);
        });
        TextCommand = MakeAsyncCommand(async () =>
        {
            using var loading = dialog.Loading("0");

            for (var i = 0; i <= 100; i++)
            {
                loading.Update($"{i}");
                await Task.Delay(50);
            }
        });
    }
}
