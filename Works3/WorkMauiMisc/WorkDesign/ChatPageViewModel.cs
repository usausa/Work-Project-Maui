namespace WorkDesign;

using System.Windows.Input;

using Smart.Maui.ViewModels;
using Smart.Mvvm;

using System.Collections.ObjectModel;

public partial class ChatPageViewModel : ExtendViewModelBase
{
    public ObservableCollection<ChatMessage> ChatMessages { get; } = [];

    [ObservableProperty]
    public string? Status { get; set; }

    [ObservableProperty]
    public string? Message { get; set; }

    public ICommand SendCommand { get; }

    public ChatPageViewModel()
    {
        SendCommand = MakeAsyncCommand(SendMessage);

        InitializeData();
    }

    private void InitializeData()
    {
        ChatMessages.Add(new ChatMessage
        {
            Type = MessageType.Send,
            DateTime = DateTime.Now.AddDays(-1),
            Author = "You",
            TextContent = "This is a sent message sample."
        });
        ChatMessages.Add(new ChatMessage
        {
            Type = MessageType.Receive,
            DateTime = DateTime.Now.AddDays(-1).AddMinutes(3),
            Author = "Alice",
            TextContent = "This is a received message sample."
        });
        ChatMessages.Add(new ChatMessage
        {
            Type = MessageType.System,
            DateTime = DateTime.Today
        });
        ChatMessages.Add(new ChatMessage
        {
            Type = MessageType.Send,
            DateTime = DateTime.Now.AddMinutes(-1),
            Author = "You",
            TextContent = "This is a resent message."
        });
    }

    private async Task SendMessage()
    {
        if (String.IsNullOrEmpty(Message))
        {
            return;
        }

        ChatMessages.Add(new ChatMessage
        {
            Type = MessageType.Send,
            DateTime = DateTime.Now,
            Author = "You",
            TextContent = Message
        });

        Status = "Echo is messaging...";

        await Task.Delay(1000); // Simulate network delay

        Status = string.Empty;

        ChatMessages.Add(new ChatMessage()
        {
            Type = MessageType.Receive,
            Author = "Echo",
            TextContent = $"Echo: {ChatMessages.Last().TextContent}"
        });
    }
}

public enum MessageType
{
    Send,
    Receive,
    System
}

public sealed class ChatMessage
{
    public MessageType Type { get; set; }

    public DateTime DateTime { get; set; }

    public string Author { get; set; } = default!;

    public string TextContent { get; set; } = default!;

    // TODO Stamp
}

public sealed class ChatMessageTemplateSelector : DataTemplateSelector
{
    public DataTemplate? SendMessage { get; set; }

    public DataTemplate? ReceiveMessage { get; set; }

    public DataTemplate? SystemMessage { get; set; }

    protected override DataTemplate? OnSelectTemplate(object item, BindableObject container)
    {
        return ((ChatMessage)item).Type switch
        {
            MessageType.Send => SendMessage,
            MessageType.Receive => ReceiveMessage,
            MessageType.System => SystemMessage,
            _ => null
        };
    }
}
