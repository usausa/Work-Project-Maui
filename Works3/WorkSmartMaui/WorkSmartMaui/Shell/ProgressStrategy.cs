namespace WorkSmartMaui.Shell;

using System;
using System.Reflection.Metadata;
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
    private readonly ProgressConfig config;

    private readonly Timer timer;

    private IProgressStrategyCallback? callback;

    //private float progress;
    private float start;

    public CircleProgressStrategy(ProgressConfig config)
    {
        this.config = config;
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
        //progress = 0;
        start = 0;
        timer.Start();
    }

    public void Detach()
    {
        timer.Stop();
        callback = null;
    }

    private void TimerOnElapsed(object? sender, ElapsedEventArgs e)
    {
        //　TODOここも設定
        start += 3f;
        if (start >= 360f)
        {
            start -= 360f;
        }
        MainThread.BeginInvokeOnMainThread(() => callback?.Invalidate());
    }

    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        // 中心座標と半径
        var cx = dirtyRect.Center.X;
        var cy = dirtyRect.Center.Y;
        var radius = config.IndicatorSize / 2;

        // アンチエイリアス
        canvas.Antialias = true;

        // --- 背景の360°円を白で描画 ---
        canvas.StrokeColor = config.IndicatorColor;
        canvas.StrokeSize = 4;
        // DrawArc で 0°→360° の円を描く
        canvas.DrawCircle(cx, cy, radius);
        //canvas.DrawArc(
        //    cx - radius, cy - radius,
        //    radius * 2, radius * 2,
        //    0f, 360f,
        //    false, false);

        // --- インジケーター本体（90°の弧） ---
        canvas.StrokeColor = Colors.White;
        canvas.StrokeSize = 4;
        canvas.StrokeLineCap = LineCap.Round;

        // 開始角度を正規化し、90°分のスイープ角度を計算
        var startAngle = start;
        var sweep = 30f;
        var endAngle = startAngle + sweep;

        if (endAngle < 360)
        //if (endAngle <= 360f)
        {
            // 360°を超えない場合
            canvas.DrawArc(
                cx - radius, cy - radius,
                radius * 2, radius * 2,
                startAngle, endAngle,
                false, false);
        }
        else
        {
            // 360° をまたぐ場合はふたつに分割して描画
            // 1) startAngle → 360°
            canvas.DrawArc(
                cx - radius, cy - radius,
                radius * 2, radius * 2,
                startAngle, 360f,
                false, false);
            // 2) 0° → (endAngle - 360f)
            canvas.DrawArc(
                cx - radius, cy - radius,
                radius * 2, radius * 2,
                0f, endAngle - 360f,
                false, false);
        }

        //// 中心へ移動して回転
        //var centerX = dirtyRect.Center.X;
        //var centerY = dirtyRect.Center.Y;
        //canvas.Translate(centerX, centerY);
        //canvas.Rotate(start);

        //// 描画スタイル
        //canvas.StrokeColor = Colors.DodgerBlue;
        //canvas.StrokeSize = 8;
        //canvas.StrokeLineCap = LineCap.Round;

        //// 半径を計算
        //float radius = 32;

        //// 中心から外周に向かう線分を描画
        //// (0, -radius) から (0, -radius + 長さ) へ
        //float lineLength = 40;
        //canvas.DrawLine(0, -radius, 0, -radius + lineLength);

        //Style = SKPaintStyle.Stroke,
        //Color = SKColors.DeepSkyBlue,    // 青い色
        //StrokeCap = SKStrokeCap.Round

        // Calculate center and radius
        //var cx = width / 2f;
        //    var cy = height / 2f;
        //    var radius = Math.Min(width, height) / 2f - paint.StrokeWidth * 1.5f;

        //     Define the spinner arc (270° sweep)
        //    var startAngle = _angle;
        //    var sweepAngle = 270f;

        //     Convert to SKRect
        //    var rect = new SKRect(
        //        cx - radius,
        //        cy - radius,
        //        cx + radius,
        //        cy + radius);

        //     Draw the arc
        //    canvas.DrawArc(rect, startAngle, sweepAngle, false, paint);
        //}

        //var size = Math.Min(dirtyRect.Width, dirtyRect.Height) * 0.8f;
        //var cx = dirtyRect.Center.X;
        //var cy = dirtyRect.Center.Y;
        //var radius = size / 2;

        //// Back circle
        //canvas.StrokeColor = Colors.LightGray;
        //canvas.StrokeSize = 8;
        //canvas.DrawCircle(cx, cy, radius);

        //// Loading circle
        //canvas.StrokeColor = Colors.Blue;
        //canvas.StrokeSize = 8;
        //var sweepAngle = 270;
        //var startAngle = (progress * 360) % 360;
        //canvas.DrawArc(cx - radius, cy - radius, radius * 2, radius * 2, startAngle, sweepAngle, false, false);
    }
}
