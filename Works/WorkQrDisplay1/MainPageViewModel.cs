namespace WorkQrDisplay1;

using System.Windows.Input;

using Smart.ComponentModel;
using Smart.Maui.ViewModels;

public class MainPageViewModel : ViewModelBase
{
    private int counter;

    public NotificationValue<string> Text { get; } = new();

    public ICommand UpdateCommand { get; }

    public MainPageViewModel()
    {
        UpdateCommand = MakeDelegateCommand(() => Text.Value = $"{++counter:D8}");
    }
}
