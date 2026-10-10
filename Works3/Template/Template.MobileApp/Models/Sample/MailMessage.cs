namespace Template.MobileApp.Models.Sample;

public enum MailFolder
{
    Inbox,
    Sent,
    Draft,
    Trash
}

public sealed partial class MailMessage : ObservableObject
{
    public DateTime DateTime { get; set; }

    public ImageSource? Image { get; set; }

    public string From { get; set; } = default!;

    public string Title { get; set; } = default!;

    public string Body { get; set; } = default!;

    // 添付のファイル名
    public string? Attachment { get; set; }

    public bool HasAttachment => Attachment is not null;

    // 一覧の要約 (改行を詰める)
    public string Preview => string.Join(' ', Body.Split('\n', StringSplitOptions.RemoveEmptyEntries));

    [ObservableProperty]
    public partial MailFolder Folder { get; set; }

    [ObservableProperty]
    public partial bool IsUnread { get; set; }

    [ObservableProperty]
    public partial bool IsStarred { get; set; }
}
