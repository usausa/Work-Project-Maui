namespace WorkRepeat;

using Smart.Maui.Input;
using Smart.Maui.ViewModels;

using System.Globalization;
using System.Windows.Input;

public class MainPageViewModel : ExtendViewModelBase
{
    private bool executing;

    public IObserveCommand RepeatCommand { get; }

    public MainPageViewModel()
    {
        RepeatCommand = MakeAsyncCommand(async () =>
        {
            RepeatCommand!.RaiseCanExecuteChanged();

            if (executing)
            {
                System.Diagnostics.Debug.WriteLine("* B");
                return;
            }

            executing = true;

            System.Diagnostics.Debug.WriteLine("* S");
            await Task.Delay(1000);
            System.Diagnostics.Debug.WriteLine("* E");

            executing = false;
        }, () => !BusyState.IsBusy);
    }
}
