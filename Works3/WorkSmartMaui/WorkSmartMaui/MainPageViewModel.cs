namespace WorkSmartMaui;

using System.Diagnostics;
using System.Windows.Input;

using Smart.Maui.Messaging;
using Smart.Mvvm;

internal partial class MainPageViewModel : AppViewModelBase
{
    public FocusController FocusController { get; } = new();

    [ObservableProperty]
    public partial bool Enable { get; set; }

    public ICommand BusyCommand { get; }

    public ICommand EnableCommand { get; }

    public ICommand Focus1Command { get; }
    public ICommand Focus2Command { get; }

    public ICommand ErrorCommand { get; }

    public MainPageViewModel()
    {
        BusyCommand = MakeAsyncCommand(async () =>
        {
            Debug.WriteLine("* Start");
            await Task.Delay(5000);
            Debug.WriteLine("* End");
        });

        EnableCommand = MakeDelegateCommand(() => Enable = !Enable);

        Focus1Command = MakeDelegateCommand(() => FocusController.FocusRequest("Target1"));
        Focus2Command = MakeAsyncCommand(async () =>
        {
            await Task.Delay(1000);
            FocusController.FocusRequest("Target2");

            // Test
            Dispatcher.GetForCurrentThread()?.DispatchDelayed(TimeSpan.FromMilliseconds(50), () =>
            {
                var focused = FocusController.FindRequest();
                Debug.WriteLine($"* Focused {focused}");
            });
        });
    }
}
