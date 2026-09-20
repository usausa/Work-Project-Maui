namespace ChatConsole;

using System.Globalization;
using System.Speech.Recognition;

// Windows の音声認識 (System.Speech のディクテーション)。アプリの ISpeechService (端末の音声認識) に相当し、
// 1 回の発話を認識して無音が続くと自動で終わる。途中経過は Hypothesis に入る
internal sealed class SpeechRecognizer : IDisposable
{
    private readonly SpeechRecognitionEngine engine;

    private TaskCompletionSource<string?>? completion;

    private string hypothesis = string.Empty;

    public string Hypothesis => Volatile.Read(ref hypothesis);

    private SpeechRecognizer(SpeechRecognitionEngine engine)
    {
        this.engine = engine;
        engine.SpeechHypothesized += (_, e) => Volatile.Write(ref hypothesis, e.Result.Text);
        engine.RecognizeCompleted += (_, e) => completion?.TrySetResult(e.Cancelled || (e.Error is not null) ? null : e.Result?.Text);
    }

    // 認識エンジンが無い (言語の音声認識が未インストール) / 既定の録音デバイスが無いときは null
    public static SpeechRecognizer? TryCreate(CultureInfo culture, out string reason)
    {
        SpeechRecognitionEngine? engine = null;
        try
        {
            engine = new SpeechRecognitionEngine(culture);
            engine.LoadGrammar(new DictationGrammar());
            engine.SetInputToDefaultAudioDevice();
            engine.InitialSilenceTimeout = TimeSpan.FromSeconds(10);
            engine.EndSilenceTimeout = TimeSpan.FromSeconds(1.5);
            engine.EndSilenceTimeoutAmbiguous = TimeSpan.FromSeconds(1.5);

            reason = string.Empty;
            return new SpeechRecognizer(engine);
        }
        catch (Exception ex) when (ex is ArgumentException or InvalidOperationException or PlatformNotSupportedException)
        {
            engine?.Dispose();
            reason = ex.Message;
            return null;
        }
    }

    public void Dispose()
    {
        engine.RecognizeAsyncCancel();
        engine.Dispose();
    }

    // 認識した文字列。認識できなければ null
    public Task<string?> RecognizeAsync()
    {
        Volatile.Write(ref hypothesis, string.Empty);
        completion = new TaskCompletionSource<string?>(TaskCreationOptions.RunContinuationsAsynchronously);
        engine.RecognizeAsync(RecognizeMode.Single);
        return completion.Task;
    }

    // 停止 (それまでの発話で確定する)
    public void Stop() => engine.RecognizeAsyncStop();

    public void Cancel() => engine.RecognizeAsyncCancel();
}
