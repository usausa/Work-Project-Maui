using System.Diagnostics;

namespace WorkVisualMusic;

#if ANDROID
using Android.Media;
using Android.Media.Audiofx;

using MauiComponents;
#endif


public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
    }

#if ANDROID
    private readonly Service service = new();
#endif

    private async void Button_OnClicked(object? sender, EventArgs e)
    {
        var status = await Permissions.CheckStatusAsync<Permissions.Microphone>();
        if (status != PermissionStatus.Granted)
        {
            status = await Permissions.RequestAsync<Permissions.Microphone>();
        }

#if ANDROID
        await service.Start();
#endif
    }
}

#if ANDROID
public sealed class Service
{
    private MediaPlayer? player;
    private Visualizer? visualizer;


    public async ValueTask<bool> Start()
    {
        if (player is not null)
        {
            return false;
        }

        var path = Path.Combine(FileSystem.CacheDirectory, "test.mp3");
        if (!File.Exists(path))
        {
            await using var input = await FileSystem.OpenAppPackageFileAsync("test.mp3");
            await using var output = File.Create(path);
            await input.CopyToAsync(output);
        }

        // TODO
        player = new MediaPlayer(Android.App.Application.Context);
        await player.SetDataSourceAsync(path);
        player.Looping = true;
        // ReSharper disable once MethodHasAsyncOverload
        player.Prepare();

        player.Start();

        // Visualizer
        var audioSessionId = player.AudioSessionId;
        visualizer = new Visualizer(audioSessionId);

        var captureSizeRange = Visualizer.GetCaptureSizeRange();
        var captureSize = captureSizeRange?.ElementAtOrDefault(1) ?? 0;
        visualizer.SetCaptureSize(captureSize);

        visualizer.SetDataCaptureListener(new VisualizerDataCaptureListener(), Visualizer.MaxCaptureRate / 2, true, true);

        visualizer.SetEnabled(true);

        return true;
    }

    private class VisualizerDataCaptureListener : Java.Lang.Object, Visualizer.IOnDataCaptureListener
    {
        public void OnWaveFormDataCapture(Visualizer? visualizer, byte[]? waveform, int samplingRate)
        {
            if (waveform is null)
            {
                return;
            }

            //var sum = 0d;
            //// ReSharper disable once ForCanBeConvertedToForeach
            //for (var i = 0; i < waveform.Length; i++)
            //{
            //    var value = waveform[i] - 128;
            //    sum += Math.Abs(value);
            //}

            //var max = waveform.Max();
            //var min = waveform.Min();

            //var avgAmplitude = sum / waveform.Length;


            //Debug.WriteLine($"*WaveForm : {min} {max} {avgAmplitude} {ConvertToVolumeLevel(avgAmplitude)}");
            //Debug.WriteLine($"*WaveForm : {AudioLevelCalculator.ConvertToLevel16(wa)}");
        }

        private const int LevelCount = 16;

        private int ConvertToVolumeLevel(double amplitude)
        {
            // 波形データの振幅は0〜127の範囲
            // 実際には20〜50程度の値が多いので、それに合わせて調整

            // 対数スケールでの変換
            var normalized = Math.Log10(1 + amplitude / 10) / 1.5;
            normalized = Math.Min(1.0, Math.Max(0, normalized));

            // 0〜15の範囲に変換
            var level = (int)Math.Floor(normalized * LevelCount);
            return Math.Min(LevelCount - 1, Math.Max(0, level));
        }

        public void OnFftDataCapture(Visualizer? visualizer, byte[]? fft, int samplingRate)
        {
            //Debug.WriteLine($"*FFT : {fft?.Length} {samplingRate}");
        }
    }
}
#endif

public class AudioLevelCalculator
{
    public static int ConvertToLevel16(byte[] waveformData, bool useLogarithmic = true)
    {
        // RMS値を計算
        float rms = CalculateVolumeLevel(waveformData);

        if (useLogarithmic)
        {
            // 対数スケール（dB）に変換して16段階に正規化
            float db = ConvertToDecibels(rms);
            float normalized = NormalizeDecibels(db);
            return (int)(normalized * 15);
        }
        else
        {
            // 線形スケールで16段階に変換
            // 理論上の最大RMS値は128
            float normalized = rms / 128.0f;
            return (int)(normalized * 15);
        }
    }

    // 波形データから音量レベルを計算
    public static float CalculateVolumeLevel(byte[] waveformData)
    {
        if (waveformData == null || waveformData.Length == 0)
            return 0;

        float sum = 0;
        for (int i = 0; i < waveformData.Length; i++)
        {
            // 128を中心として偏差を計算
            float sample = waveformData[i] - 128;
            // 二乗値を加算
            sum += sample * sample;
        }

        // 平均を取って平方根（RMS値）
        return (float)Math.Sqrt(sum / waveformData.Length);
    }

    // RMS値をdB（デシベル）に変換
    public static float ConvertToDecibels(float rms)
    {
        // 無音（非常に小さい値）の場合は最小値を返す
        if (rms < 0.001f)
            return -90.0f; // 最小dB値

        // RMSをdBに変換（20 * log10(rms/reference)）
        // リファレンス値は128（理論上の最大振幅）
        return 20 * (float)Math.Log10(rms / 128.0f);
    }

    // dB値を0-1の範囲に正規化（UIの表示用）
    public static float NormalizeDecibels(float db)
    {
        // 一般的なオーディオの表示範囲（例：-60dB 〜 0dB）
        const float MIN_DB = -60.0f;
        const float MAX_DB = 0.0f;

        // 範囲内に収める
        db = Math.Max(MIN_DB, Math.Min(MAX_DB, db));

        // 0〜1の範囲に正規化
        return (db - MIN_DB) / (MAX_DB - MIN_DB);
    }
}