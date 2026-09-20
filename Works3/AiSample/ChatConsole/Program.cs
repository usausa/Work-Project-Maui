using System.Text;

using ChatConsole;

using Microsoft.Extensions.Configuration;

using OllamaSharp;

using Template.MobileApp.State;

// 設定は appsettings.json < 環境変数 (AISAMPLE_ 接頭辞) < コマンドライン (--endpoint / --model) の順で上書き
var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: true)
    .AddEnvironmentVariables("AISAMPLE_")
    .AddCommandLine(args, CommandLine.Switches)
    .Build();

var settings = new Settings
{
    OllamaEndPoint = configuration["OllamaEndPoint"] ?? string.Empty,
    OllamaModel = configuration["OllamaModel"] ?? string.Empty
};

Console.OutputEncoding = Encoding.UTF8;
Terminal.WriteLine("ChatConsole - Ollama (IChatClient) / 音声入力");
Terminal.WriteLine($"EndPoint : {settings.OllamaEndPoint}");
Terminal.WriteLine($"Model    : {settings.OllamaModel}");
Terminal.WriteLine("/voice で音声入力、/exit で終了");
Terminal.WriteLine();

// 未設定は実行しない (アプリはメニューで同じ文言を案内して画面に入らない)
if (String.IsNullOrEmpty(settings.OllamaModel) || !Uri.TryCreate(settings.OllamaEndPoint, UriKind.Absolute, out var endpoint))
{
    Terminal.WriteLine("Ollama end point is not configured.");
    return;
}

using var chatClient = new OllamaApiClient(endpoint, settings.OllamaModel);
var session = new ChatSession(chatClient);

// 冒頭のあいさつはアプリと同じ
Terminal.WriteLine($"ai> こんにちは!AI アシスタントです。開発に関する質問をどうぞ 🤖\n(Ollama: {settings.OllamaModel})");

// 音声入力で認識した文章 (入力欄に反映した状態)。空のまま Enter で送信する
string? applied = null;
while (true)
{
    var input = Terminal.Prompt(applied is null ? "you> " : $"you [{applied}]> ");
    if (input is null)
    {
        break;
    }

    if (input.Length == 0)
    {
        if (applied is null)
        {
            continue;
        }

        input = applied;
    }

    applied = null;

    if (input.Equals("/exit", StringComparison.OrdinalIgnoreCase) || input.Equals("/quit", StringComparison.OrdinalIgnoreCase))
    {
        break;
    }

    if (input.Equals("/voice", StringComparison.OrdinalIgnoreCase))
    {
        applied = await VoiceInput.RunAsync().ConfigureAwait(false);
        if (applied is not null)
        {
            Terminal.WriteLine("入力欄に反映しました。そのまま Enter で送信します");
        }

        continue;
    }

    await session.SendAsync(input).ConfigureAwait(false);
}
