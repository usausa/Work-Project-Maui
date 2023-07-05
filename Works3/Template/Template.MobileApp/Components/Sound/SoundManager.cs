namespace Template.MobileApp.Components.Sound;

public interface ISoundManager
{
    // Speech

    ValueTask SpeakAsync(string text, float? pitch = null, float? volume = null);

    void SpeakCancel();
}

public sealed class SoundManager : ISoundManager, IDisposable
{
    private readonly ITextToSpeech textToSpeech;

    private CancellationTokenSource? cts;

    public SoundManager(ITextToSpeech textToSpeech)
    {
        this.textToSpeech = textToSpeech;
    }

    public void Dispose()
    {
        cts?.Dispose();
    }

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
}
