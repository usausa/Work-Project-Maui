namespace Template.MobileApp.Models.Sample.Chat;

public sealed partial class ChatMessage : ObservableObject
{
    public MessageType Type { get; set; }

    public DateTime DateTime { get; init; } = DateTime.Now;

    public string Author { get; set; } = default!;

    public string TextContent { get; set; } = default!;

    public string? AvatarSource { get; set; }

    public string? StampSource { get; set; }

    public string? PhotoSource { get; set; }

    public bool IsRead { get; set; }

    public ObservableCollection<MessageReaction> Reactions { get; init; } = [];

    [ObservableProperty]
    public partial bool IsSelected { get; set; }

    // 同じ絵文字があれば数を足し、無ければ 1 で加える
    public void AddReaction(string emoji)
    {
        var reaction = Reactions.FirstOrDefault(x => x.Emoji == emoji);
        if (reaction is not null)
        {
            reaction.Count++;
            return;
        }

        Reactions.Add(new MessageReaction { Emoji = emoji, Count = 1 });
        RaisePropertyChanged(nameof(Reactions));
    }
}
