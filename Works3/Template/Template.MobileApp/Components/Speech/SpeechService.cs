namespace Template.MobileApp.Components.Speech;

using CommunityToolkit.Maui.Media;

public interface ISpeechService
{
    // Text to speech

    ValueTask SpeakAsync(string text, float? pitch = null, float? volume = null);

    void SpeakCancel();

    // Speech to text

    ValueTask<string?> RecognizeAsync(Action<string> progress, CancellationToken cancel = default);
}

public sealed class SpeechService : ISpeechService, IDisposable
{
    private readonly ITextToSpeech textToSpeech;

    private readonly ISpeechToText speechToText;

    private CancellationTokenSource? cts;

    public SpeechService(
        ITextToSpeech textToSpeech,
        ISpeechToText speechToText)
    {
        this.textToSpeech = textToSpeech;
        this.speechToText = speechToText;
    }

    public void Dispose()
    {
        cts?.Dispose();
    }

    // ------------------------------------------------------------
    // Text to speech
    // ------------------------------------------------------------

    public async ValueTask SpeakAsync(string text, float? pitch, float? volume)
    {
        cts = new CancellationTokenSource();
        var options = new SpeechOptions
        {
            Pitch = pitch,
            Volume = volume
        };
        await textToSpeech.SpeakAsync(text, options, cts.Token);
    }

    public void SpeakCancel()
    {
        if (cts?.IsCancellationRequested ?? true)
        {
            return;
        }

        cts.Cancel();
    }

    // ------------------------------------------------------------
    // Speech to text
    // ------------------------------------------------------------

    public async ValueTask<string?> RecognizeAsync(Action<string> progress, CancellationToken cancel = default)
    {
        var granted = await speechToText.RequestPermissions(default);
        if (!granted)
        {
            return null;
        }

        var result = await speechToText.ListenAsync(
            CultureInfo.CurrentCulture,
            new Progress<string>(progress),
            cancel);
        result.EnsureSuccess();

        if (result.IsSuccessful)
        {
            return result.Text;
        }

        return null;
    }
}
