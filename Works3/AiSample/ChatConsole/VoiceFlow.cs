namespace ChatConsole;

using System.Diagnostics;
using System.Globalization;

using Microsoft.Extensions.AI;

using OllamaSharp.Models.Exceptions;

// 音声フローの抽出プレビュー項目
internal sealed record VoiceExtractItem(string Label, string Value);

// 音声入力の 4 ステップ (録音 → 文字起こし → 抽出 → 確認)。文字起こしは Windows の音声認識 (文字での入力も可)、
// 項目の抽出は IChatClient に依頼する (未設定 / 音声なしは固定の例)。抽出の依頼と読み取りはアプリの SampleChatViewModel と同じ
internal sealed class VoiceFlow
{
    private static readonly string[] ExtractLabels = ["種別", "期日", "対象", "参照"];

    private static readonly VoiceExtractItem[] MockExtractItems =
    [
        new("種別", "依頼"),
        new("期日", "明日 15:00"),
        new("対象", "ログイン画面の不具合修正"),
        new("参照", "共有済みのチケット")
    ];

    private static readonly char[] Separators = [':', '：'];

    private const string Choices = "\rrq";

    private readonly IChatClient? chatClient;

    private readonly string model;

    private readonly bool useSpeech;

    public VoiceFlow(IChatClient? chatClient, string model, bool useSpeech)
    {
        this.chatClient = chatClient;
        this.model = model;
        this.useSpeech = useSpeech;
    }

    // 確認まで進んで適用した文章。中止は null
    public async Task<string?> RunAsync()
    {
        while (true)
        {
            // Step 1: 録音
            Terminal.WriteLine("[1/4] 録音");
            var (transcribedText, transcribed) = await RecordAsync().ConfigureAwait(false);
            if (transcribedText is null)
            {
                return null;
            }

            // Step 2: 文字起こし
            Terminal.WriteLine("[2/4] 文字起こし");
            Terminal.WriteLine(transcribedText);
            var choice = Terminal.ReadChoice("Enter: 抽出へ / r: やり直し / q: 中止> ", Choices);
            if (choice == 'r')
            {
                continue;
            }

            if (choice == 'q')
            {
                return null;
            }

            // Step 3: 抽出
            Terminal.WriteLine("[3/4] 抽出");
            var items = await ExtractAsync(transcribedText, transcribed).ConfigureAwait(false);
            foreach (var item in items)
            {
                Terminal.WriteLine($"{item.Label}: {item.Value}");
            }

            choice = Terminal.ReadChoice("Enter: 確認へ / r: やり直し / q: 中止> ", Choices);
            if (choice == 'r')
            {
                continue;
            }

            if (choice == 'q')
            {
                return null;
            }

            // Step 4: 確認
            Terminal.WriteLine("[4/4] 確認");
            Terminal.WriteLine(transcribedText);
            choice = Terminal.ReadChoice("Enter: 適用 (入力欄へ反映) / r: やり直し / q: 中止> ", Choices);
            if (choice == 'r')
            {
                continue;
            }

            if (choice == 'q')
            {
                return null;
            }

            return transcribedText;
        }
    }

    // 文字起こしの結果と、音声認識で実際に文字が得られたか (得られていなければ抽出は固定の例)。中止は null
    private async Task<(string? Text, bool Transcribed)> RecordAsync()
    {
        var choice = useSpeech
            ? Terminal.ReadChoice("Enter: 録音開始 / t: 文字で入力 / q: 中止> ", "\rtq")
            : Terminal.ReadChoice("Enter: 文字で入力 / q: 中止> ", "\rq");
        if (choice == 'q')
        {
            return (null, false);
        }

        if (!useSpeech || (choice == 't'))
        {
            var text = Terminal.Prompt("文章> ");
            return String.IsNullOrEmpty(text) ? (null, false) : (text, true);
        }

        using var recognizer = SpeechRecognizer.TryCreate(CultureInfo.CurrentCulture, out var reason);
        if (recognizer is null)
        {
            Terminal.WriteLine(reason);
            return ("(音声認識を開始できませんでした)", false);
        }

        // 認識は無音が続くと自動で終わる。Enter は停止 (それまでの発話で確定)
        var task = recognizer.RecognizeAsync();
        var watch = Stopwatch.StartNew();
        while (!task.IsCompleted)
        {
            if (Terminal.IsEnterPressed())
            {
                recognizer.Stop();
                break;
            }

            Terminal.Rewrite($"録音中 {(int)watch.Elapsed.TotalSeconds} 秒 (Enter で停止、無音で自動停止) {recognizer.Hypothesis}");
            await Task.Delay(200).ConfigureAwait(false);
        }

        var recognized = await task.ConfigureAwait(false);
        Terminal.EndRewrite();
        return String.IsNullOrEmpty(recognized) ? ("(音声を認識できませんでした)", false) : (recognized, true);
    }

    // 抽出は IChatClient に「項目: 値」の 4 行で答えさせて読み取る
    private async Task<VoiceExtractItem[]> ExtractAsync(string transcribedText, bool transcribed)
    {
        if (chatClient is null)
        {
            Terminal.WriteLine("内容から以下を抽出しました (Ollama 未設定のため固定の例)");
            return MockExtractItems;
        }

        if (!transcribed)
        {
            Terminal.WriteLine("内容から以下を抽出しました (音声が無いため固定の例)");
            return MockExtractItems;
        }

        Terminal.Rewrite("Ollama で抽出中...");
        try
        {
            var prompt = $"次の文章から「{String.Join("」「", ExtractLabels)}」を抜き出し、各行を「項目: 値」の形式で {ExtractLabels.Length} 行だけ出力してください。該当が無い項目の値は「-」にしてください。\n\n{transcribedText}";
            var response = await chatClient.GetResponseAsync(prompt).ConfigureAwait(false);

            Terminal.Rewrite($"内容から以下を抽出しました (Ollama: {model})");
            Terminal.EndRewrite();
            return ParseExtractItems(response.Text);
        }
        catch (Exception ex) when (ex is HttpRequestException or OllamaException or OperationCanceledException)
        {
            Terminal.Rewrite($"抽出できませんでした (固定の例を表示)。{ex.Message}");
            Terminal.EndRewrite();
            return MockExtractItems;
        }
    }

    private static VoiceExtractItem[] ParseExtractItems(string text)
    {
        var values = new Dictionary<string, string>(StringComparer.Ordinal);
        foreach (var line in text.Split('\n'))
        {
            var index = line.IndexOfAny(Separators);
            if (index <= 0)
            {
                continue;
            }

            var label = line[..index].Trim().Trim('「', '」', '*', '-', ' ');
            var value = line[(index + 1)..].Trim();
            if (ExtractLabels.Contains(label))
            {
                values.TryAdd(label, value);
            }
        }

        return ExtractLabels.Select(x => new VoiceExtractItem(x, values.GetValueOrDefault(x, "-"))).ToArray();
    }
}
