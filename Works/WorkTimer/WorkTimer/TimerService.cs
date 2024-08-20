namespace WorkTimer;

public class TimerService
{
    public event EventHandler<EventArgs> Handle;

    private PeriodicTimer? timer;
    private CancellationTokenSource? cts;
    private Task? task;

    public TimeSpan Interval { get; set; } = TimeSpan.FromSeconds(1);

    public bool IsRunning => timer is not null;

    public ValueTask StartAsync()
    {
        if (IsRunning)
        {
            return ValueTask.CompletedTask;
        }

        cts = new CancellationTokenSource();
        timer = new PeriodicTimer(Interval);

        task = Task.Run(async () =>
        {
            try
            {
                while (await timer.WaitForNextTickAsync(cts.Token))
                {
                    Handle?.Invoke(this, EventArgs.Empty);
                }
            }
            catch (OperationCanceledException)
            {
            }
        });

        return ValueTask.CompletedTask;
    }

    public async ValueTask StopAsync()
    {
        if (!IsRunning)
        {
            return;
        }

        // ReSharper disable once MethodHasAsyncOverload
        cts!.Cancel();
        await task!;
        timer = null;
        cts = null;
    }
}
