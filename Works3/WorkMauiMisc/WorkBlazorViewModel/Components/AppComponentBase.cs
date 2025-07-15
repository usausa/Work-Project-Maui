namespace WorkBlazorViewModel.Components;

using System.Reactive.Disposables;

using Microsoft.AspNetCore.Components;

using Smart.Maui.Messaging;
using Smart.Mvvm.ViewModels;

public abstract class AppComponentBase : ComponentBase, IDisposable
{
    private CompositeDisposable? disposables;

    protected ICollection<IDisposable> Disposables => disposables ??= [];

    [Inject]
    public required IReactiveMessenger Messenger { get; set; }


    [Inject]
    public required IBusyState BusyState { get; set; }


    ~AppComponentBase()
    {
        Dispose(false);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            disposables?.Dispose();
        }
    }

    protected void Execute(Action func)
    {
        if (BusyState.IsBusy)
        {
            return;
        }

        using (BusyState.Begin())
        {
            // TODO StateHasChanged()

            func();
        }
        // TODO StateHasChanged()
    }

    protected void Execute<TParameter>(TParameter parameter, Action<TParameter> func)
    {
        if (BusyState.IsBusy)
        {
            return;
        }

        using (BusyState.Begin())
        {
            func(parameter);
        }
    }

    protected async Task ExecuteAsync(Func<Task> func)
    {
        if (BusyState.IsBusy)
        {
            return;
        }

        using (BusyState.Begin())
        {
            await func().ConfigureAwait(true);
        }
    }


    protected async Task ExecuteAsync<TParameter>(TParameter parameter, Func<TParameter, Task> func)
    {
        if (BusyState.IsBusy)
        {
            return;
        }

        using (BusyState.Begin())
        {
            await func(parameter).ConfigureAwait(true);
        }
    }


    protected async Task<TResult> ExecuteAsync<TResult>(Func<Task<TResult>> func)
    {
        if (BusyState.IsBusy)
        {
            return default!;
        }

        using (BusyState.Begin())
        {
            return await func().ConfigureAwait(true);
        }
    }


    protected async Task<TResult> ExecuteAsync<TParameter, TResult>(TParameter parameter, Func<TParameter, Task<TResult>> func)
    {
        if (BusyState.IsBusy)
        {
            return default!;
        }

        using (BusyState.Begin())
        {
            return await func(parameter).ConfigureAwait(true);
        }
    }
}
