namespace OverlayExample;

using System.Windows.Input;

using Smart.Maui.ViewModels;

public class MainPageViewModel : ViewModelBase
{
    public ICommand LoadingCommand { get; }

    public MainPageViewModel(IDialog dialog)
    {
        LoadingCommand = MakeAsyncCommand(async () =>
        {
            using var loading = dialog.Loading();

            await Task.Delay(3000);
        });
    }
}
