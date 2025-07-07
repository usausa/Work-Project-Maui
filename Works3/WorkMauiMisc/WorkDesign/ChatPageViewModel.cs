using System.Windows.Input;

namespace WorkDesign;

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
    }



    private async Task SendMessage()
    {
        if (String.IsNullOrEmpty(Message))
        {
            return;
        }

        await Task.Delay(1000); // Simulate network delay
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

    public DateTime DateTime { get; } = DateTime.Now;

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
