namespace WorkSmartMaui.Shell;

using System;
using System.Timers;

// TODO config?

public sealed class CircleProgressStrategy : IProgressStrategy, IDisposable
{
    private readonly Timer timer;

    private IProgressStrategyCallback? callback;

    private float progress;

    public CircleProgressStrategy()
    {
        timer = new Timer(16);
        timer.Elapsed += TimerOnElapsed;
        timer.Enabled = false;
    }

    public void Dispose()
    {
        timer.Dispose();
    }

    public void Attach(IProgressStrategyCallback value)
    {
        callback = value;
        progress = 0;
        timer.Start();
    }

    public void Detach()
    {
        timer.Stop();
        callback = null;
    }

    private void TimerOnElapsed(object? sender, ElapsedEventArgs e)
    {
        progress += 0.01f;
        if (progress > 1f)
        {
            progress = 0;
        }
        MainThread.BeginInvokeOnMainThread(() => callback?.Invalidate());
    }


    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        var size = Math.Min(dirtyRect.Width, dirtyRect.Height) * 0.8f;
        var cx = dirtyRect.Center.X;
        var cy = dirtyRect.Center.Y;
        var radius = size / 2;

        // Back circle
        canvas.StrokeColor = Colors.LightGray;
        canvas.StrokeSize = 8;
        canvas.DrawCircle(cx, cy, radius);

        // Loading circle
        canvas.StrokeColor = Colors.Blue;
        canvas.StrokeSize = 8;
        var sweepAngle = 270;
        var startAngle = (progress * 360) % 360;
        canvas.DrawArc(cx - radius, cy - radius, radius * 2, radius * 2, startAngle, sweepAngle, false, false);
    }
}
