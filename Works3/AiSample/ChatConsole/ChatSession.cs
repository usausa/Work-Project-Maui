namespace ChatConsole;

using System.Text;

using Microsoft.Extensions.AI;

using OllamaSharp.Models.Exceptions;

// 会話。応答は IChatClient のストリーミング (履歴ごと送り、応答を履歴に足す)、未設定のときは固定文の疑似応答。
// 処理はアプリの SampleChatViewModel の SendAsync / RespondByChatClientAsync / RespondByMockAsync と同じ
internal sealed class ChatSession
{
    private static readonly (string Text, bool IsCode)[] Replies =
    [
        ("なるほど、良い質問ですね。.NET MAUI では XAML でレイアウトを宣言し、データバインディングで ViewModel と接続します。コードビハインドを使わずに Behavior や Trigger で振る舞いを追加するのがおすすめです。", false),
        ("その場合は BindableProperty を定義してコントロールに公開します。例を書いてみますね。", false),
        ("public sealed class GreetingService\n{\n    public string CreateMessage(string name)\n    {\n        ArgumentNullException.ThrowIfNull(name);\n        return $\"Hello, {name}! Welcome to .NET MAUI.\";\n    }\n}", true),
        ("補足すると、リスト表示には CollectionView を使い、ItemsUpdatingScrollMode を KeepLastItemInView にするとチャットのように末尾へ追従します。パフォーマンスが必要な場面では DataTemplateSelector でテンプレートを分けるのが定石です。", false)
    ];

    private const string Typing = "ai> ...";

    private readonly IChatClient? chatClient;

    // 会話の履歴 (IChatClient はステートレスのため呼び出し側で保持する)
    private readonly List<ChatMessage> history = [];

    private int replyIndex;

    public ChatSession(IChatClient? chatClient)
    {
        this.chatClient = chatClient;
    }

    public Task SendAsync(string text) =>
        chatClient is not null ? RespondByChatClientAsync(chatClient, text) : RespondByMockAsync();

    // 応答をストリーミングで流し込む (履歴ごと送り、応答を履歴に足す)
    private async Task RespondByChatClientAsync(IChatClient target, string text)
    {
        Terminal.Rewrite(Typing);

        history.Add(new ChatMessage(ChatRole.User, text));
        var builder = new StringBuilder();
        try
        {
            await foreach (var update in target.GetStreamingResponseAsync(history).ConfigureAwait(false))
            {
                if (builder.Length == 0)
                {
                    Terminal.Rewrite("ai> ");
                }

                builder.Append(update.Text);
                Terminal.Write(update.Text);
            }

            Terminal.EndRewrite();
            history.Add(new ChatMessage(ChatRole.Assistant, builder.ToString()));
        }
        catch (Exception ex) when (ex is HttpRequestException or OllamaException or OperationCanceledException)
        {
            history.RemoveAt(history.Count - 1);
            Terminal.EndRewrite();
            Terminal.WriteLine($"応答を取得できませんでした。\n{ex.Message}");
        }
    }

    // タイピングインジケータを表示してから応答をストリーミング風に流し込む
    private async Task RespondByMockAsync()
    {
        var (reply, isCode) = Replies[replyIndex % Replies.Length];
        replyIndex++;

        Terminal.Rewrite(Typing);
        await Task.Delay(1200).ConfigureAwait(false);
        Terminal.Rewrite("ai> ");
        if (isCode)
        {
            Terminal.WriteLine();
        }

        for (var i = 0; i < reply.Length; i += 3)
        {
            Terminal.Write(reply[i..Math.Min(i + 3, reply.Length)]);
            await Task.Delay(30).ConfigureAwait(false);
        }

        Terminal.EndRewrite();
    }
}
