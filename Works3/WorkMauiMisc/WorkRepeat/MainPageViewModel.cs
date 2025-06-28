namespace WorkRepeat;

using Smart.Maui.Input;
using Smart.Maui.ViewModels;

public class MainPageViewModel : ExtendViewModelBase
{
    private bool executing;

    public IObserveCommand RepeatCommand { get; }

    public MainPageViewModel()
    {
        RepeatCommand = MakeAsyncCommand(async () =>
        {
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
