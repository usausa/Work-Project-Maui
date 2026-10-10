namespace Template.MobileApp.Models.Sample.Chat;

public sealed partial class MessageReaction : ObservableObject
{
    public string Emoji { get; set; } = default!;

    [ObservableProperty]
    public partial int Count { get; set; }
}
