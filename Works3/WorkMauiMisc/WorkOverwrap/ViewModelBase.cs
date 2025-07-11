namespace Smart.Mvvm.ViewModels;

using System.ComponentModel;

using Smart.Mvvm;
using Smart.Mvvm.Messaging;

public interface IViewModelOptions
{
    IBusyState BusyState { get; }

    IMessenger Messenger { get; }
}

public class ViewModelOptions : IViewModelOptions
{
    public IBusyState BusyState { get; set; } = Smart.Mvvm.ViewModels.BusyState.Default;

    public IMessenger Messenger { get; set; } = Smart.Mvvm.Messaging.Messenger.Default;
}

#pragma warning disable IDE0032
// ReSharper disable ReplaceWithFieldKeyword
public abstract class ViewModelBase2 : ObservableObject, IDisposable
{
    private static readonly PropertyChangedEventArgs ErrorsChangedEventArgs = new(nameof(Errors));

    private static IViewModelOptions DefaultOptions { get; } = new ViewModelOptions();


    private Disposables? disposables;

    // ------------------------------------------------------------
    // Disposables
    // ------------------------------------------------------------

    protected Disposables Disposables => disposables ??= new Disposables();

    // ------------------------------------------------------------
    // ErrorInfo
    // ------------------------------------------------------------

    public ErrorInfo Errors { get; } = new();

    // ------------------------------------------------------------
    // Busy
    // ------------------------------------------------------------

    public IBusyState BusyState { get; }

    // ------------------------------------------------------------
    // Messenger
    // ------------------------------------------------------------

    public IMessenger Messenger { get; }

    // ------------------------------------------------------------
    // Constructor
    // ------------------------------------------------------------

    protected ViewModelBase2(IViewModelOptions? options = null)
    {
        options ??= DefaultOptions;
        BusyState = options.BusyState;
        Messenger = options.Messenger;
    }

    ~ViewModelBase2()
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
            disposables = null;

            Errors.Dispose();
        }
    }

    private void OnErrorChanged()
    {
        RaisePropertyChanged(ErrorsChangedEventArgs);
    }
}
