using Smart.Mvvm;

namespace WorkTimer;

using Smart.Mvvm.ViewModels;

internal partial class MainPageViewModel : ViewModelBase
{
    [ObservableProperty] public partial int Value { get; set; }

    private readonly PeriodicTimer timer;
    private readonly CancellationTokenSource cancellationTokenSource;

    public MainPageViewModel()
    {
        timer = new PeriodicTimer(TimeSpan.FromMilliseconds(1000d / 60));
        cancellationTokenSource = new CancellationTokenSource();
        Disposables.Add(timer);
        Disposables.Add(cancellationTokenSource);

        _ = StartTimerAsync();
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            cancellationTokenSource.Cancel();
        }

        base.Dispose(disposing);
    }

    private async Task StartTimerAsync()
    {
        try
        {
            while (await timer.WaitForNextTickAsync(cancellationTokenSource.Token))
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    Value++;
                });
            }
        }
        catch (OperationCanceledException)
        {
            // Ignore
        }
    }
}