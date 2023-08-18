namespace Template.MobileApp.Messaging;

public class EntryCompleteEvent
{
    public bool HasError { get; set; }
}

public interface IEntryMessenger : INotifyPropertyChanged
{
    event EventHandler<EventArgs> FocusRequested;

    string? Text { get; set; }

    bool Enable { get; set; }

    void HandleCompleted(EntryCompleteEvent e);
}

public sealed class EntryMessenger : NotificationObject, IEntryMessenger
{
    private event EventHandler<EventArgs>? Requested;

    private readonly ICommand? command;

    private string? text;

    private bool enable;

    // Property

    public string? Text
    {
        get => text;
        set => SetProperty(ref text, value);
    }

    public bool Enable
    {
        get => enable;
        set => SetProperty(ref enable, value);
    }

    // Constructor

    public EntryMessenger()
    {
        enable = true;
    }

    public EntryMessenger(bool enable)
    {
        this.enable = enable;
    }

    public EntryMessenger(ICommand command)
    {
        enable = true;
        this.command = command;
    }

    public EntryMessenger(bool enable, ICommand command)
    {
        this.enable = enable;
        this.command = command;
    }

    // Request

    event EventHandler<EventArgs> IEntryMessenger.FocusRequested
    {
        add => Requested += value;
        remove => Requested -= value;
    }

    public void FocusRequest()
    {
        Requested?.Invoke(this, EventArgs.Empty);
    }

    // Event

    void IEntryMessenger.HandleCompleted(EntryCompleteEvent e)
    {
        if ((command is not null) && command.CanExecute(e))
        {
            command.Execute(e);
        }
    }
}
