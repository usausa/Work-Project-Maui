using System.Diagnostics;

namespace WorkDecibel;

using Android.Media;

public partial class MainPage : ContentPage
{
    private AudioAnalyzer analyzer = new();

    public MainPage()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        await CheckAndRequestAsync();
    }

    public static async Task<bool> CheckAndRequestAsync()
    {
        var status = await Permissions.CheckStatusAsync<Permissions.Microphone>();
        if (status != PermissionStatus.Granted)
        {
            status = await Permissions.RequestAsync<Permissions.Microphone>();
        }
        return status == PermissionStatus.Granted;
    }

    private void Button_OnClicked(object? sender, EventArgs e)
    {
        analyzer.StartMeasure(x =>
        {
            Debug.WriteLine($"* {x:F2}");
        });
    }
}

public sealed class AudioAnalyzer
{
    private AudioRecord _audioRecord;
    private bool _isMeasuring;
    private Task _measureTask;

    public void StartMeasure(Action<double> onDbMeasured)
    {
        int sampleRate = 44100;
        int bufferSize = AudioRecord.GetMinBufferSize(sampleRate, ChannelIn.Mono, Encoding.Pcm16bit);

        _audioRecord = new AudioRecord(AudioSource.Mic, sampleRate, ChannelIn.Mono, Encoding.Pcm16bit, bufferSize);
        _audioRecord.StartRecording();
        _isMeasuring = true;

        _measureTask = Task.Run(async () =>
        {
            short[] buffer = new short[bufferSize];
            while (_isMeasuring)
            {
                int read = await _audioRecord.ReadAsync(buffer, 0, buffer.Length);
                double sum = 0;
                for (int i = 0; i < read; i++)
                {
                    sum += buffer[i] * buffer[i];
                }
                if (read > 0)
                {
                    double rms = Math.Sqrt(sum / read);
                    double db = 20 * Math.Log10(rms == 0 ? 1 : rms);
                    onDbMeasured?.Invoke(db);
                }
                await Task.Delay(200);
            }
        });
    }

    public void StopMeasure()
    {
        _isMeasuring = false;
        _audioRecord?.Stop();
        _audioRecord?.Release();
        _audioRecord = null;
    }
}