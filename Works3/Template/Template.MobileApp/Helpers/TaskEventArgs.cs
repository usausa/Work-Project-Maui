namespace Template.MobileApp.Helpers;

public abstract class TaskEventArgs : EventArgs
{
    public Task Task { get; set; } = Task.CompletedTask;
}

public abstract class TaskEventArgs<T> : EventArgs
{
    private static readonly Task<T> CompletedTask = System.Threading.Tasks.Task.FromResult(default(T)!);

    public Task<T> Task { get; set; } = CompletedTask;
}
