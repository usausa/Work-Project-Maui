namespace WorkSmartMaui;

using System.Diagnostics;
using System.Windows.Input;

using Smart.Mvvm;
using Smart.Mvvm.Messaging;

internal partial class MainPageViewModel : AppViewModelBase
{
    public EventRequest<string> FocusRequest { get; } = new();

    [ObservableProperty]
    public partial bool Enable { get; set; }

    public ICommand BusyCommand { get; }

    public ICommand EnableCommand { get; }

    public ICommand Focus1Command { get; }

    public ICommand Focus2Command { get; }

    public MainPageViewModel()
    {
        BusyCommand = MakeAsyncCommand(async () =>
        {
            Debug.WriteLine("* Start");
            await Task.Delay(5000);
            Debug.WriteLine("* End");
        });

        EnableCommand = MakeDelegateCommand(() => Enable = !Enable);

        Focus1Command = MakeDelegateCommand(() => FocusRequest.Request("Target1"));
        Focus2Command = MakeAsyncCommand(async () =>
        {
            await Task.Delay(1000);
            FocusRequest.Request("Target2");
        });
    }
}
