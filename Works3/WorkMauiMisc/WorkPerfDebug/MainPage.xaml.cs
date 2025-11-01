namespace WorkPerfDebug;

#if ANDROID
using Android.Views;
#else
#endif

using System.Diagnostics;

public partial class MainPage : ContentPage
{
    private readonly Stopwatch stopwatch;
    private readonly int processorCount = Environment.ProcessorCount;
    private readonly Process currentProcess = Process.GetCurrentProcess();

    private TimeSpan cpuTimePrev;

    private long allocatedBytesPrev;

    private int gc0Prev;
    private int gc1Prev;
    private int gc2Prev;

    private IDisplayManager? displayManager;

    private const double MinFrameTime = 0.1;
    private const double EmaAlpha = 0.9;
    private double emaFrameTime = 0;
    private double emaFps = 0;

    int count = 0;

    public MainPage()
    {
        InitializeComponent();

        cpuTimePrev = currentProcess.TotalProcessorTime;
        allocatedBytesPrev = GC.GetTotalAllocatedBytes();

        stopwatch = new Stopwatch();
        stopwatch.Start();

#if ANDROID
        displayManager = new DisplayManager();
        displayManager.FrameUpdated += (frameTimeMs) =>
        {
            frameTimeMs = Math.Max(frameTimeMs, MinFrameTime);

            var fps = 1000.0 / frameTimeMs;
            emaFps = emaFps == 0 ? fps : (EmaAlpha * emaFps) + ((1 - EmaAlpha) * fps);

            emaFrameTime = emaFrameTime == 0 ? frameTimeMs : (EmaAlpha * emaFrameTime) + ((1 - EmaAlpha) * frameTimeMs);

            // TODO check main thread?
            //Debug.WriteLine($"FPS={emaFps}, FrameTime={emaFrameTime}");
        };
        displayManager.Start();
#endif

        Application.Current!.Dispatcher.StartTimer(TimeSpan.FromSeconds(1), () =>
        {
            // CPU
            var cpuTimeCurrent = currentProcess.TotalProcessorTime;
            var cpuUsage = ((cpuTimeCurrent - cpuTimePrev).TotalMilliseconds / stopwatch.Elapsed.TotalMilliseconds) * 100 / processorCount;
            cpuTimePrev = cpuTimeCurrent;

            // Thread
            var threads = currentProcess.Threads.Count;

            // Memory
            var memory = currentProcess.WorkingSet64 / (1024 * 1024);

            // Allocation
            var elapsedSec = stopwatch.Elapsed.TotalSeconds;
            if (elapsedSec <= 0)
            {
                elapsedSec = 1; // fallback
            }

            var currentAllocated = GC.GetTotalAllocatedBytes();
            var allocatedPerSec = ((currentAllocated - allocatedBytesPrev) / (1024.0 * 1024.0)) / elapsedSec; // MB/sec
            allocatedBytesPrev = currentAllocated;

            var gen0 = GC.CollectionCount(0);
            var gen1 = GC.CollectionCount(1);
            var gen2 = GC.CollectionCount(2);
            var gc0Delta = gen0 - gc0Prev;
            var gc1Delta = gen1 - gc1Prev;
            var gc2Delta = gen2 - gc2Prev;
            gc0Prev = gen0;
            gc1Prev = gen1;
            gc2Prev = gen2;

            // Display
            Debug.WriteLine($"{DateTime.Now:HH:mm:ss.fff} FPS={emaFps:F2}, FrameTime={emaFrameTime:F2}, CPU={cpuUsage:F1}%, Threads={threads}, Memory={memory}MB, Alloc={allocatedPerSec:F2}MB/s, GC:Gen0={gc0Delta}, GC:Gen1={gc1Delta}, GC:Gen2={gc2Delta}");

            stopwatch.Restart();

            return true;
        });
    }

    private void OnCounterClicked(object? sender, EventArgs e)
    {
        count++;

        if (count == 1)
            CounterBtn.Text = $"Clicked {count} time";
        else
            CounterBtn.Text = $"Clicked {count} times";

        SemanticScreenReader.Announce(CounterBtn.Text);
    }
}

public interface IDisplayManager
{
    public event Action<double> FrameUpdated;

    void Start();

    void Stop();
}

#if ANDROID
public sealed class DisplayManager : Java.Lang.Object, Choreographer.IFrameCallback, IDisplayManager
{
    public event Action<double>? FrameUpdated;

    private long lastFrameTimeNanosecond;

    public void Start()
    {
        lastFrameTimeNanosecond = 0;
        Choreographer.Instance?.PostFrameCallback(this);
    }

    public void Stop()
    {
        Choreographer.Instance?.RemoveFrameCallback(this);
    }

    public void DoFrame(long frameTimeNanos)
    {
        if (lastFrameTimeNanosecond > 0)
        {
            double frameTimeMs = (frameTimeNanos - lastFrameTimeNanosecond) / 1_000_000.0;
            FrameUpdated?.Invoke(frameTimeMs);
        }
        lastFrameTimeNanosecond = frameTimeNanos;

        Choreographer.Instance!.PostFrameCallback(this);
    }
}
#endif