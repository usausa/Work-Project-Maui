using System.Diagnostics;

namespace WorkSmartMaui;

using System.Windows.Input;

internal class MainPageViewModel : AppViewModelBase
{
    public ICommand BusyCommand { get; }

    public MainPageViewModel()
    {
        BusyCommand = MakeAsyncCommand(async () =>
        {
            Debug.WriteLine("* Start");
            await Task.Delay(5000);
            Debug.WriteLine("* End");
        });
    }
}
