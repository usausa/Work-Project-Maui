namespace ChatConsole;

using System.Text;

using Microsoft.Extensions.AI;

using OllamaSharp.Models.Exceptions;

// 会話。応答は IChatClient のストリーミング (履歴ごと送り、応答を履歴に足す)。処理はアプリの SampleChatViewModel の RespondAsync と同じ
internal sealed class ChatSession
{
    private const string Typing = "ai> ...";

    private readonly IChatClient chatClient;

    // 会話の履歴 (IChatClient はステートレスのため呼び出し側で保持する)
    private readonly List<ChatMessage> history = [];

    public ChatSession(IChatClient chatClient)
    {
        this.chatClient = chatClient;
    }

    // 応答をストリーミングで流し込む (履歴ごと送り、応答を履歴に足す)
    public async Task SendAsync(string text)
    {
        Terminal.Rewrite(Typing);

        history.Add(new ChatMessage(ChatRole.User, text));
        var builder = new StringBuilder();
        try
        {
            await foreach (var update in chatClient.GetStreamingResponseAsync(history).ConfigureAwait(false))
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
}
