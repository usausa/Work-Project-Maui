namespace Template.MobileApp.Shell;

using System;
using System.Timers;

// ------------------------------------------------------------
// Indicator
// ------------------------------------------------------------

public sealed class IndicatorProgressStrategy : IProgressStrategy, IDisposable
{
    private readonly ProgressConfig config;

    private readonly Timer timer;

    private IProgressStrategyCallback? callback;

    private float start;

    public IndicatorProgressStrategy(ProgressConfig config)
    {
        this.config = config;

        timer = new Timer(1000d / 60);
        timer.Elapsed += TimerOnElapsed;
        timer.Enabled = false;
    }

    public void Dispose()
    {
        timer.Dispose();
    }

    void IProgressStrategy.Attach(IProgressStrategyCallback value)
    {
        callback = value;
        start = 0;
        timer.Start();
    }

    void IProgressStrategy.Detach()
    {
        timer.Stop();
        callback = null;
    }

    private void TimerOnElapsed(object? sender, ElapsedEventArgs e)
    {
        start += config.IndicatorSpeed;
        if (start >= 360f)
        {
            start -= 360f;
        }
        MainThread.BeginInvokeOnMainThread(() => callback?.Invalidate());
    }

    void IProgressStrategy.Draw(ICanvas canvas, RectF dirtyRect)
    {
        var cx = dirtyRect.Center.X;
        var cy = dirtyRect.Center.Y;
        var radius = config.IndicatorRadius;

        canvas.Antialias = true;

        canvas.StrokeSize = config.IndicatorWidth;
        canvas.StrokeColor = config.IndicatorColor1;

        canvas.DrawCircle(cx, cy, radius);

        canvas.StrokeColor = config.IndicatorColor2;
        canvas.StrokeLineCap = LineCap.Round;

        var startAngle = start;
        var sweep = config.IndicatorSweep;
        var endAngle = startAngle + sweep;

        if (endAngle < 360)
        {
            canvas.DrawArc(cx - radius, cy - radius, radius * 2, radius * 2, startAngle, endAngle, false, false);
        }
        else
        {
            canvas.DrawArc(cx - radius, cy - radius, radius * 2, radius * 2, startAngle, 360f, false, false);
            canvas.DrawArc(cx - radius, cy - radius, radius * 2, radius * 2, 0f, endAngle - 360f, false, false);
        }
    }
}
// ------------------------------------------------------------
// Message
// ------------------------------------------------------------

public sealed class MessageProgressStrategy : IProgressStrategy, IMessageProgress
{
    private readonly ProgressConfig config;

    private IProgressStrategyCallback? callback;

    private string message = string.Empty;

    public MessageProgressStrategy(ProgressConfig config)
    {
        this.config = config;
    }

    void IProgressStrategy.Attach(IProgressStrategyCallback value)
    {
        callback = value;
        message = string.Empty;
    }

    void IProgressStrategy.Detach()
    {
        callback = null;
    }

    void IProgressStrategy.Draw(ICanvas canvas, RectF dirtyRect)
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

        canvas.Antialias = true;

        canvas.FillColor = config.MessageBackgroundColor;
        canvas.FillRoundedRectangle(messageRect, config.MessageCornerRadius);

        canvas.FontColor = config.MessageColor;
        canvas.FontSize = config.MessageFontSize;
        canvas.DrawString(message, messageRect, HorizontalAlignment.Center, VerticalAlignment.Center);
    }

    void IMessageProgress.Update(string value)
    {
        message = value;
        callback?.Invalidate();
    }
}

// ------------------------------------------------------------
// Rate
// ------------------------------------------------------------

public sealed class RateProgressStrategy : IProgressStrategy, IRateProgress
{
    private readonly ProgressConfig config;

    private IProgressStrategyCallback? callback;

    private double rate;

    public RateProgressStrategy(ProgressConfig config)
    {
        this.config = config;
    }

    void IProgressStrategy.Attach(IProgressStrategyCallback value)
    {
        callback = value;
        rate = 0;
    }

    void IProgressStrategy.Detach()
    {
        callback = null;
    }

    void IProgressStrategy.Draw(ICanvas canvas, RectF dirtyRect)
    {
        var arcRect = new RectF(
            (dirtyRect.Width / 2) - config.RateSize,
            (dirtyRect.Height / 2) - config.RateSize,
            config.RateSize * 2,
            config.RateSize * 2);

        canvas.Antialias = true;

        canvas.FillColor = config.RateAreaBackgroundColor;
        canvas.FillRoundedRectangle(
            new RectF(
                (dirtyRect.Width / 2) - config.RateAreaSize,
                (dirtyRect.Height / 2) - config.RateAreaSize,
                config.RateAreaSize * 2,
                config.RateAreaSize * 2),
            config.RateAreaCornerRadius);

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

    void IRateProgress.Update(double value)
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
