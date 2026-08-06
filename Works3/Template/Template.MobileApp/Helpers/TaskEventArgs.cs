namespace Template.MobileApp.Helpers;

public abstract class ValueTaskEventArgs : EventArgs
{
    public ValueTask Task { get; set; } = ValueTask.CompletedTask;
}

public abstract class ValueTaskEventArgs<T> : EventArgs
{
// ValueTaskはイベント引数で中継され呼び出し元で一度だけawaitされるため抑止
#pragma warning disable CA2012
    public ValueTask<T> Task { get; set; } = ValueTask.FromResult(default(T)!);
#pragma warning restore CA2012
}
