namespace ChatConsole;

using System.Diagnostics;
using System.Globalization;

// 音声入力。Windows の音声認識で 1 回の発話を認識して文章を返す (アプリの ChatView のマイクボタンに相当)
internal static class VoiceInput
{
    // 認識した文章。認識できなければ null
    public static async Task<string?> RunAsync()
    {
        using var recognizer = SpeechRecognizer.TryCreate(CultureInfo.CurrentCulture, out var reason);
        if (recognizer is null)
        {
            Terminal.WriteLine(reason);
            return null;
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

            Terminal.Rewrite($"認識中 {(int)watch.Elapsed.TotalSeconds} 秒 (Enter で停止、無音で自動停止) {recognizer.Hypothesis}");
            await Task.Delay(200).ConfigureAwait(false);
        }

        var recognized = await task.ConfigureAwait(false);
        Terminal.EndRewrite();
        if (String.IsNullOrEmpty(recognized))
        {
            Terminal.WriteLine("音声を認識できませんでした");
            return null;
        }

        return recognized;
    }
}
