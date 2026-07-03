namespace Template.MobileApp.Modules.Sample;

using System.Collections.ObjectModel;

using Template.MobileApp.Models.Sample.Chat;

public sealed partial class SampleChatViewModel : AppViewModelBase
{
    private static readonly (string Text, bool IsCode)[] Replies =
    [
        ("なるほど、良い質問ですね。.NET MAUI では XAML でレイアウトを宣言し、データバインディングで ViewModel と接続します。コードビハインドを使わずに Behavior や Trigger で振る舞いを追加するのがおすすめです。", false),
        ("その場合は BindableProperty を定義してコントロールに公開します。例を書いてみますね。", false),
        ("public sealed class GreetingService\n{\n    public string CreateMessage(string name)\n    {\n        ArgumentNullException.ThrowIfNull(name);\n        return $\"Hello, {name}! Welcome to .NET MAUI.\";\n    }\n}", true),
        ("補足すると、リスト表示には CollectionView を使い、ItemsUpdatingScrollMode を KeepLastItemInView にするとチャットのように末尾へ追従します。パフォーマンスが必要な場面では DataTemplateSelector でテンプレートを分けるのが定石です。", false),
    ];

    private int replyIndex;

    private bool responding;

    [ObservableProperty]
    public partial string InputText { get; set; } = string.Empty;

    public ObservableCollection<AiChatMessage> Messages { get; } = [];

    public IObserveCommand SendCommand { get; }

    public SampleChatViewModel()
    {
        SendCommand = MakeAsyncCommand(SendAsync, () => !responding && !string.IsNullOrWhiteSpace(InputText));
        PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(InputText))
            {
                SendCommand.RaiseCanExecuteChanged();
            }
        };
    }

    public override Task OnNavigatedToAsync(INavigationContext context)
    {
        if (Messages.Count == 0)
        {
            Messages.Add(new AiChatMessage
            {
                Role = AiChatRole.Assistant,
                Text = "こんにちは!AI アシスタントです。開発に関する質問をどうぞ 🤖",
            });
        }
        return Task.CompletedTask;
    }

    private async Task SendAsync()
    {
        var text = InputText.Trim();
        InputText = string.Empty;
        Messages.Add(new AiChatMessage { Role = AiChatRole.User, Text = text });

        responding = true;
        SendCommand.RaiseCanExecuteChanged();
        try
        {
            var (reply, isCode) = Replies[replyIndex % Replies.Length];
            replyIndex++;

            // タイピングインジケータを表示してから応答をストリーミング風に流し込む
            var message = new AiChatMessage { Role = AiChatRole.Assistant, IsCode = isCode, IsTyping = true };
            Messages.Add(message);

            await Task.Delay(1200).ConfigureAwait(true);
            message.IsTyping = false;

            for (var i = 0; i < reply.Length; i += 3)
            {
                message.Text = reply[..Math.Min(i + 3, reply.Length)];
                await Task.Delay(30).ConfigureAwait(true);
            }
        }
        finally
        {
            responding = false;
            SendCommand.RaiseCanExecuteChanged();
        }
    }

    protected override Task OnNotifyBackAsync() => Navigator.ForwardAsync(ViewId.SampleMenu);

    protected override Task OnNotifyFunction1() => OnNotifyBackAsync();
}
