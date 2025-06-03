namespace WorkVisualRadar;

using Microsoft.Maui.Graphics;

using System;

#pragma warning disable CA1001
public sealed class RadarControl : GraphicsView, IDrawable
{
    private const float SweepLength = 60f;

    private static readonly TimeSpan Interval = TimeSpan.FromMilliseconds(1000d / 60);

    private CancellationTokenSource? cts;

    private float currentAngle;

    public RadarControl()
    {
        Drawable = this;
    }

    protected override void OnHandlerChanged()
    {
        base.OnHandlerChanged();

        if (Handler != null)
        {
            StartTimer();
        }
        else
        {
            StopTimer();
        }
    }

    private void StartTimer()
    {
        if ((cts is not null) && !cts.IsCancellationRequested)
        {
            return;
        }

        cts = new CancellationTokenSource();
        _ = Loop(cts.Token);
    }

    private void StopTimer()
    {
        if (cts is null)
        {
            return;
        }

        cts.Cancel();
        cts.Dispose();
        cts = null;
    }

    private async Task Loop(CancellationToken ct)
    {
        try
        {
            using var timer = new PeriodicTimer(Interval);
            while (await timer.WaitForNextTickAsync(ct))
            {
                if (ct.IsCancellationRequested)
                {
                    break;
                }

                MainThread.BeginInvokeOnMainThread(() =>
                {
                    currentAngle += 1.5f;
                    if (currentAngle >= 360f)
                    {
                        currentAngle -= 360f;
                    }

                    Invalidate();
                });
            }
        }
        catch (OperationCanceledException)
        {
            // Ignore
        }
    }

    private static float DegreesToRadians(float degrees) =>
        degrees * ((float)Math.PI / 180f);

    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        var width = dirtyRect.Width;
        var height = dirtyRect.Height;
        var cx = width / 2f;
        var cy = height / 2f;
        var radius = Math.Min(cx, cy) * 0.9f;

        canvas.Antialias = true;

        // Background
        canvas.FillColor = Colors.Black;
        canvas.FillRectangle(0, 0, width, height);

        // Circle
        canvas.StrokeColor = Colors.Green;
        canvas.StrokeSize = 2;

        // Outer
        canvas.DrawEllipse(cx - radius, cy - radius, radius * 2, radius * 2);
        // Inner
        for (var i = 1; i <= 3; i++)
        {
            var r = radius * i / 3f;
            canvas.DrawEllipse(cx - r, cy - r, r * 2, r * 2);
        }

        // Cross line
        for (var a = 0; a < 360; a += 45)
        {
            var rad = DegreesToRadians(a);
            var x = cx + radius * (float)Math.Cos(rad);
            var y = cy + radius * (float)Math.Sin(rad);
            canvas.DrawLine(cx, cy, x, y);
        }
        // Memory
        for (var a = 0; a < 360; a += 5)
        {
            var rad = DegreesToRadians(a);
            var outer = radius;
            var inner = radius - ((a % 10) == 0 ? 16 : 8);
            var x1 = cx + outer * (float)Math.Cos(rad);
            var y1 = cy + outer * (float)Math.Sin(rad);
            var x2 = cx + inner * (float)Math.Cos(rad);
            var y2 = cy + inner * (float)Math.Sin(rad);
            canvas.DrawLine(x1, y1, x2, y2);
        }

        // Sweep arc
        canvas.FillColor = new Color(0, 255, 0, 12);

        var endAngle = 360f - currentAngle;
        for (var i = 0; i < SweepLength; i+= 2)
        {
            var startAngle = (endAngle + i) % 360f;

            var path = new PathF();
            path.MoveTo(cx, cy);
            path.AddArc(cx - radius, cy - radius, cx + radius, cy + radius, startAngle, endAngle, true);
            path.Close();

            canvas.FillPath(path);
        }

        // Sweep line
        canvas.StrokeColor = Colors.Lime;
        canvas.StrokeSize = 3;

        var radCurrent = DegreesToRadians(currentAngle);
        var ex = cx + radius * (float)Math.Cos(radCurrent);
        var ey = cy + radius * (float)Math.Sin(radCurrent);

        canvas.DrawLine(cx, cy, ex, ey);
    }
}
#pragma warning restore CA1001
