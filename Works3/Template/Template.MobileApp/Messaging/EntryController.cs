namespace Template.MobileApp.Messaging;

using Template.MobileApp.Helpers;

public sealed class EntryCompleteEvent
{
    public bool Handled { get; set; }
}

public interface IEntryController : INotifyPropertyChanged
{
    // Property

    string? Text { get; set; }

    bool Enable { get; set; }

    // Attach

    void Attach(Entry view);

    void Detach();
}

public sealed class EntryController : NotificationObject, IEntryController
{
    // Field

    private readonly ICommand? command;

    private Entry? entry;

    // Property

    public string? Text
    {
        get;
        set => SetProperty(ref field, value);
    }

    public bool Enable
    {
        get;
        set => SetProperty(ref field, value);
    }

    // Constructor

    public EntryController()
    {
        Enable = true;
    }

    public EntryController(bool enable)
    {
        Enable = enable;
    }

    public EntryController(ICommand command)
    {
        Enable = true;
        this.command = command;
    }

    public EntryController(bool enable, ICommand command)
    {
        Enable = enable;
        this.command = command;
    }

    // Attach

    void IEntryController.Attach(Entry view)
    {
        entry = view;
        view.Completed += HandleCompleted;
    }

    void IEntryController.Detach()
    {
        if (entry is not null)
        {
            entry.Completed -= HandleCompleted;
        }
        entry = null;
    }

    // Request

    public void FocusRequest()
    {
        entry?.Focus();
    }

    // Event

    private void HandleCompleted(object? sender, EventArgs e)
    {
        var ice = new EntryCompleteEvent();
        if ((command is not null) && command.CanExecute(ice))
        {
            command.Execute(ice);
            if (!ice.Handled)
            {
                ElementHelper.MoveFocusInRoot((Entry)sender!, true);
            }
        }
    }
}
