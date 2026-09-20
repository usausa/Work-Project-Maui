using System.Text;

using ChatConsole;

using Microsoft.Extensions.Configuration;

using Template.MobileApp.Services;
using Template.MobileApp.State;

// 設定は appsettings.json < 環境変数 (AISAMPLE_ 接頭辞) < コマンドライン (--endpoint / --model / --speech) の順で上書き。
// モデルを空 (--model "") にすると疑似応答で動く。--speech false は音声認識を使わず文字で入力する
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
var useSpeech = !Boolean.TryParse(configuration["Speech"], out var speech) || speech;

var factory = new AiChatClientFactory(settings);
using var chatClient = factory.Create();
var session = new ChatSession(chatClient);
var voice = new VoiceFlow(chatClient, factory.Model, useSpeech);

Console.OutputEncoding = Encoding.UTF8;
Terminal.WriteLine("ChatConsole - Ollama (IChatClient) / 音声入力");
Terminal.WriteLine($"EndPoint : {settings.OllamaEndPoint}");
Terminal.WriteLine($"Model    : {(String.IsNullOrEmpty(settings.OllamaModel) ? "(未設定)" : settings.OllamaModel)}");
Terminal.WriteLine($"Speech   : {(useSpeech ? "音声認識" : "文字で入力")}");
Terminal.WriteLine("/voice で音声入力、/exit で終了");
Terminal.WriteLine();

// 冒頭のあいさつはアプリと同じ (動作モードを表示)
Terminal.WriteLine(chatClient is null
    ? "ai> こんにちは!AI アシスタントです。開発に関する質問をどうぞ 🤖\n(Ollama が未設定のため疑似応答で動作します)"
    : $"ai> こんにちは!AI アシスタントです。開発に関する質問をどうぞ 🤖\n(Ollama: {factory.Model})");

// 音声フローで適用した文章 (入力欄に反映した状態)。空のまま Enter で送信する
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
        applied = await voice.RunAsync().ConfigureAwait(false);
        if (applied is not null)
        {
            Terminal.WriteLine("入力欄に反映しました。そのまま Enter で送信します");
        }

        continue;
    }

    await session.SendAsync(input).ConfigureAwait(false);
}
