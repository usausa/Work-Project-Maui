using System.Diagnostics;
using System.Windows.Input;
using CommunityToolkit.Maui.Media;

namespace WorkMaui;

using System.Globalization;

using Smart.Maui.ViewModels;

internal class MainPageViewModel : ViewModelBase
{
    public ICommand StartCommand { get; }

    public ICommand StopCommand { get; }

    public MainPageViewModel()
    {
        var service = new SpeechService(TextToSpeech.Default, SpeechToText.Default);
        StartCommand = MakeAsyncCommand(async () => await service.StartRecognizeAsync());
        StopCommand = MakeAsyncCommand(async () => await service.StopRecognizeAsync());

        var ret = OperatingSystem.IsAndroidVersionAtLeast(31);
    }
}

//--------------------------------------------------------------------------------




//--------------------------------------------------------------------------------

//public interface ISpeechService
//{
//    // Text to speech

//    ValueTask SpeakAsync(string text, float? pitch = null, float? volume = null);

//    void SpeakCancel();

//    // Speech to text

//    ValueTask<string?> RecognizeAsync(Progress<string> progress, CultureInfo? culture = null, CancellationToken cancel = default);
//}

public sealed class SpeechService : IDisposable
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
        this.speechToText.StateChanged += SpeechToTextOnStateChanged;
        this.speechToText.RecognitionResultUpdated += SpeechToTextOnRecognitionResultUpdated;
        this.speechToText.RecognitionResultCompleted += SpeechToTextOnRecognitionResultCompleted;
    }

    private void SpeechToTextOnStateChanged(object? sender, SpeechToTextStateChangedEventArgs e)
    {
        Debug.WriteLine("StateChanged: " + e.State);
    }

    private void SpeechToTextOnRecognitionResultUpdated(object? sender, SpeechToTextRecognitionResultUpdatedEventArgs e)
    {
        Debug.WriteLine("RecognitionResultUpdated: " + e.RecognitionResult);
    }

    private void SpeechToTextOnRecognitionResultCompleted(object? sender, SpeechToTextRecognitionResultCompletedEventArgs e)
    {
        Debug.WriteLine("RecognitionResultCompleted: " + e.RecognitionResult);
    }

    public void Dispose()
    {
        cts?.Dispose();
        speechToText.StateChanged -= SpeechToTextOnStateChanged;
        speechToText.RecognitionResultUpdated -= SpeechToTextOnRecognitionResultUpdated;
        speechToText.RecognitionResultCompleted -= SpeechToTextOnRecognitionResultCompleted;
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
        await textToSpeech.SpeakAsync(text, options, cts.Token).ConfigureAwait(true);
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

    public async ValueTask StartRecognizeAsync(CultureInfo? culture = null, bool shouldReportPartialResults = true)
    {
        var granted = await speechToText.RequestPermissions().ConfigureAwait(true);
        if (!granted)
        {
            return;
        }

        await speechToText.StartListenAsync(new SpeechToTextOptions
        {
            Culture = culture ?? CultureInfo.CurrentCulture,
            ShouldReportPartialResults = shouldReportPartialResults
        });
    }

    public async ValueTask StopRecognizeAsync()
    {
        await speechToText.StopListenAsync();
    }
}
