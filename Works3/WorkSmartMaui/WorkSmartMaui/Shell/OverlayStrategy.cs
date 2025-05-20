namespace WorkSmartMaui.Shell;

using System;
using System.Timers;

public interface IOverlayCallback
{
    void Invalidate();
}

public interface IOverlayStrategy
{
    void Attach(IOverlayCallback value);

    void Detach();

    void Draw(ICanvas canvas, RectF dirtyRect);
}

public sealed class CircleOverlayStrategy : IOverlayStrategy, IDisposable
{
    public static CircleOverlayStrategy Instance { get; } = new();

    private readonly Timer timer;

    private IOverlayCallback? callback;

    private float progress;

    public CircleOverlayStrategy()
    {
        timer = new Timer(16);
        timer.Elapsed += TimerOnElapsed;
        timer.Enabled = false;
    }

    public void Dispose()
    {
        timer.Dispose();
    }


    public void Attach(IOverlayCallback value)
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
        canvas.FillColor = new(255, 255, 255, 64);
        canvas.FillRectangle(dirtyRect);

        // Background
        canvas.FillColor = new(255, 255, 255, 64);
        canvas.FillRectangle(dirtyRect);

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
        canvas.DrawArc(
            cx - radius, cy - radius,
            radius * 2, radius * 2,
            startAngle, sweepAngle,
            false, false);
    }
}
