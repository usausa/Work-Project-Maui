using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Smart.Maui.ViewModels;

using System.ComponentModel;
using System.Reactive.Linq;

using Smart.Maui.Input;
using Smart.Maui.Internal;
using Smart.Mvvm.Messaging;
using Smart.Mvvm.ViewModels;

[Flags]
public enum CommandBehavior
{
    None = 0,

    ControlByBusyState = 1 << 0,

    AllowBusyExecution = 1 << 1,

    Default = 1 << 31
}


public interface IExtendViewModelOptions : IViewModelOptions
{
    CommandBehavior CommandBehavior { get; }
}

public class ExtendViewModelOptions : ViewModelOptions, IExtendViewModelOptions
{
    public CommandBehavior CommandBehavior { get; } = CommandBehavior.None;
}

#pragma warning disable IDE0032
// ReSharper disable ReplaceWithFieldKeyword
public abstract class ExtendViewModelBase2 : ViewModelBase2
{
    private static IExtendViewModelOptions DefaultOptions { get; } = new ExtendViewModelOptions();

    // ------------------------------------------------------------
    // Member
    // ------------------------------------------------------------

    private readonly CommandBehavior defaultBehavior;

    private List<IObserveCommand>? commands;

    // ------------------------------------------------------------
    // Constructor
    // ------------------------------------------------------------

    protected ExtendViewModelBase2(IExtendViewModelOptions? options = null)
        : base(options ?? DefaultOptions)
    {
        defaultBehavior = options?.CommandBehavior ?? DefaultOptions.CommandBehavior;
    }

    // ------------------------------------------------------------
    // Override
    // ------------------------------------------------------------

    protected override void RaisePropertyChanged(PropertyChangedEventArgs args)
    {
        base.RaisePropertyChanged(args);

        UpdateCommandState();
    }

    // ------------------------------------------------------------
    // Command helper
    // ------------------------------------------------------------

    private void AddCommandObserver(IObserveCommand command)
    {
        if (commands is null)
        {
            commands = new List<IObserveCommand>();
            BusyState.PropertyChanged += BusyStateOnPropertyChanged;
            Disposables.Add(new DelegateDisposable(() => BusyState.PropertyChanged -= BusyStateOnPropertyChanged));
        }
        commands.Add(command);
    }

    private void BusyStateOnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(IBusyState.IsBusy))
        {
            UpdateCommandState();
        }
    }

    private void UpdateCommandState()
    {
        if (commands is not null)
        {
            foreach (var command in commands)
            {
                command.RaiseCanExecuteChanged();
            }
        }
    }

    protected TCommand Observe<T, TCommand>(IObservable<T> observable, TCommand command)
        where TCommand : IObserveCommand
    {
        Disposables.Add(observable.Subscribe(_ => command.RaiseCanExecuteChanged()));
        return command;
    }

    protected IObserveCommand MakeDelegateCommand(Action execute, CommandBehavior behavior = CommandBehavior.Default) =>
        MakeDelegateCommand(execute, () => true, behavior);

    protected IObserveCommand MakeDelegateCommand(Action execute, Func<bool> canExecute, CommandBehavior behavior = CommandBehavior.Default)
    {
        DelegateCommand command;
        if (IsControlByBusyState(behavior))
        {
            command = new DelegateCommand(() =>
            {
                using (BusyState.Begin())
                {
                    execute();
                }
            }, () => !BusyState.IsBusy && canExecute());
        }
        else if (!IsAllowBusyExecution(behavior))
        {
            command = new DelegateCommand(() =>
            {
                if (BusyState.IsBusy)
                {
                    return;
                }

                using (BusyState.Begin())
                {
                    execute();
                }
            }, canExecute);
        }
        else
        {
            command = new DelegateCommand(() =>
            {
                using (BusyState.Begin())
                {
                    execute();
                }
            }, canExecute);
        }
        AddCommandObserver(command);
        return command;
    }

    protected IObserveCommand MakeDelegateCommand<TParameter>(Action<TParameter> execute, CommandBehavior behavior = CommandBehavior.Default) =>
        MakeDelegateCommand(execute, _ => true, behavior);

    protected IObserveCommand MakeDelegateCommand<TParameter>(Action<TParameter> execute, Func<TParameter, bool> canExecute, CommandBehavior behavior = CommandBehavior.Default)
    {
        DelegateCommand<TParameter> command;
        if (IsControlByBusyState(behavior))
        {
            command = new DelegateCommand<TParameter>(x =>
            {
                using (BusyState.Begin())
                {
                    execute(x);
                }
            }, x => !BusyState.IsBusy && canExecute(x));
        }
        else if (!IsAllowBusyExecution(behavior))
        {
            command = new DelegateCommand<TParameter>(x =>
            {
                if (BusyState.IsBusy)
                {
                    return;
                }

                using (BusyState.Begin())
                {
                    execute(x);
                }
            }, canExecute);
        }
        else
        {
            command = new DelegateCommand<TParameter>(x =>
            {
                using (BusyState.Begin())
                {
                    execute(x);
                }
            }, canExecute);
        }
        AddCommandObserver(command);
        return command;
    }

    protected IObserveCommand MakeAsyncCommand(Func<Task> execute, CommandBehavior behavior = CommandBehavior.Default) =>
        MakeAsyncCommand(execute, () => true, behavior);

    protected IObserveCommand MakeAsyncCommand(Func<Task> execute, Func<bool> canExecute, CommandBehavior behavior = CommandBehavior.Default)
    {
        AsyncCommand command;
        if (IsControlByBusyState(behavior))
        {
            command = new AsyncCommand(async () =>
            {
                using (BusyState.Begin())
                {
                    await execute().ConfigureAwait(true);
                }
            }, () => !BusyState.IsBusy && canExecute());
        }
        else if (!IsAllowBusyExecution(behavior))
        {
            command = new AsyncCommand(async () =>
            {
                if (BusyState.IsBusy)
                {
                    return;
                }

                using (BusyState.Begin())
                {
                    await execute().ConfigureAwait(true);
                }
            }, canExecute);
        }
        else
        {
            command = new AsyncCommand(async () =>
            {
                using (BusyState.Begin())
                {
                    await execute().ConfigureAwait(true);
                }
            }, canExecute);
        }
        AddCommandObserver(command);
        return command;
    }

    protected IObserveCommand MakeAsyncCommand<TParameter>(Func<TParameter, Task> execute, CommandBehavior behavior = CommandBehavior.Default) =>
        MakeAsyncCommand(execute, _ => true, behavior);

    protected IObserveCommand MakeAsyncCommand<TParameter>(Func<TParameter, Task> execute, Func<TParameter, bool> canExecute, CommandBehavior behavior = CommandBehavior.Default)
    {
        AsyncCommand<TParameter> command;
        if (IsControlByBusyState(behavior))
        {
            command = new AsyncCommand<TParameter>(async x =>
            {
                using (BusyState.Begin())
                {
                    await execute(x).ConfigureAwait(true);
                }
            }, x => !BusyState.IsBusy && canExecute(x));
        }
        else if (!IsAllowBusyExecution(behavior))
        {
            command = new AsyncCommand<TParameter>(async x =>
            {
                if (BusyState.IsBusy)
                {
                    return;
                }

                using (BusyState.Begin())
                {
                    await execute(x).ConfigureAwait(true);
                }
            }, canExecute);
        }
        else
        {
            command = new AsyncCommand<TParameter>(async x =>
            {
                using (BusyState.Begin())
                {
                    await execute(x).ConfigureAwait(true);
                }
            }, canExecute);
        }
        AddCommandObserver(command);
        return command;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool HasFlags(CommandBehavior behavior, CommandBehavior flag) =>
        (behavior & flag) == flag;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool IsControlByBusyState(CommandBehavior behavior) =>
        HasFlags(HasFlags(behavior, CommandBehavior.Default) ? defaultBehavior : behavior, CommandBehavior.ControlByBusyState);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool IsAllowBusyExecution(CommandBehavior behavior) =>
        HasFlags(HasFlags(behavior, CommandBehavior.Default) ? defaultBehavior : behavior, CommandBehavior.AllowBusyExecution);


    // ------------------------------------------------------------
    // Reactive helper
    // ------------------------------------------------------------

    protected IObservable<string?> Observe(string name)
    {
        return Observable.FromEvent<PropertyChangedEventHandler, PropertyChangedEventArgs>(
                static h => (_, e) => h(e),
                h => PropertyChanged += h,
                h => PropertyChanged -= h)
            .Where(x => x.PropertyName == name)
            .Select(x => x.PropertyName);
    }

    protected void Subscribe<T>(IObservable<T> observable, Action<T> action)
    {
        Disposables.Add(observable.Subscribe(action));
    }
}
