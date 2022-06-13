namespace LoadingExample;

using System.Windows.Input;

using Smart.ComponentModel;
using Smart.Maui.ViewModels;

public class MainPageViewModel : ViewModelBase
{
    public NotificationValue<bool> Loading { get; } = new();

    public ICommand LoadingCommand { get; }
    public ICommand DummyCommand { get; }

    public MainPageViewModel()
    {
        LoadingCommand = MakeAsyncCommand(Execute);
        DummyCommand = MakeDelegateCommand(() => System.Diagnostics.Debug.WriteLine("*"));
    }

    private async Task Execute()
    {
        try
        {
            Loading.Value = true;

            await Task.Delay(5000);
        }
        finally
        {
            Loading.Value = false;
        }
    }
}
