namespace Template.MobileApp.State;

public sealed class StartupState
{
    private readonly TaskCompletionSource completedSource = new(TaskCreationOptions.RunContinuationsAsynchronously);

    public Task Completed => completedSource.Task;

    public DateTime? CompletedAt { get; private set; }

    public void NotifyCompleted()
    {
        CompletedAt = DateTime.Now;
        completedSource.TrySetResult();
    }
}
