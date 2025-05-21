namespace WorkSmartMaui.Shell;

using System;
using System.Timers;

// Message

public interface IMessageProgress
{
    void Update(string value);
}

public sealed class MessageProgressStrategy : IProgressStrategy, IMessageProgress
{
    private readonly ProgressConfig config;

    private IProgressStrategyCallback? callback;

    private string message = string.Empty;

    public MessageProgressStrategy(ProgressConfig config)
    {
        this.config = config;
    }

    public void Attach(IProgressStrategyCallback value)
    {
        callback = value;
        message = string.Empty;
    }

    public void Detach()
    {
        callback = null;
    }

    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        if (String.IsNullOrEmpty(message))
        {
            return;
        }

        var messageRect = new RectF(
            config.MessageSideMargin,
            (dirtyRect.Height / 2) - (config.MessageHeight / 2),
            dirtyRect.Width - (config.MessageSideMargin * 2),
            config.MessageHeight);

        canvas.FillColor = config.MessageBackgroundColor;
        canvas.FillRoundedRectangle(messageRect, config.MessageCornerRadius);

        canvas.FontColor = config.MessageColor;
        canvas.FontSize = config.MessageFontSize;
        canvas.DrawString(message, messageRect, HorizontalAlignment.Center, VerticalAlignment.Center);
    }

    public void Update(string value)
    {
        message = value;
        callback?.Invalidate();
    }
}

// Rate

public interface IRateProgress
{
    void Update(double value);
}

public sealed class RateProgressStrategy : IProgressStrategy, IRateProgress
{
    private readonly ProgressConfig config;

    private IProgressStrategyCallback? callback;

    private double rate;

    public RateProgressStrategy(ProgressConfig config)
    {
        this.config = config;
    }

    public void Attach(IProgressStrategyCallback value)
    {
        callback = value;
        rate = 0;
    }

    public void Detach()
    {
        callback = null;
    }

    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        canvas.FillColor = config.RateAreaBackgroundColor;
        canvas.FillRoundedRectangle(
            new RectF(
                (dirtyRect.Width / 2) - config.RateAreaSize,
                (dirtyRect.Height / 2) - config.RateAreaSize,
                config.RateAreaSize * 2,
                config.RateAreaSize * 2),
            config.RateAreaCornerRadius);

        var arcRect = new RectF(
            (dirtyRect.Width / 2) - config.RateSize,
            (dirtyRect.Height / 2) - config.RateSize,
            config.RateSize * 2,
            config.RateSize * 2);

        canvas.StrokeSize = config.RateWidth;
        canvas.StrokeColor = config.RateCircleColor2;
        canvas.DrawArc(arcRect, 0, 360, false, false);

        canvas.StrokeColor = config.RateCircleColor1;
        var endAngle = 90 - (int)(360 * rate / 100);
        if (endAngle <= -270)
        {
            canvas.DrawArc(arcRect, 0, 360, false, false);
        }
        else
        {
            canvas.DrawArc(arcRect, 90, endAngle, true, false);
        }

        canvas.FontColor = config.RateValueColor;
        canvas.FontSize = config.RateValueFontSize;
        canvas.DrawString($"{rate:F1}%", arcRect, HorizontalAlignment.Center, VerticalAlignment.Center);
    }

    public void Update(double value)
    {
        rate = value switch
        {
            > 100 => 100,
            < 0 => 0,
            _ => value
        };
        callback?.Invalidate();
    }
}

// Circle

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
        progress += 0.02f;
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
