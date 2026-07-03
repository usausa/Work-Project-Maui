namespace Template.MobileApp.Controls;

using System.Diagnostics;

using SkiaSharp.Views.Maui;

#pragma warning disable CA1001
public abstract class AnimatedSkiaView : SKCanvasView
{
    private static readonly TimeSpan Interval = TimeSpan.FromMilliseconds(1000d / 60);

    private static readonly SKTypeface MonoTypeface = SKTypeface.FromFamilyName("monospace", SKFontStyle.Normal);
    private static readonly SKTypeface MonoBoldTypeface = SKTypeface.FromFamilyName("monospace", SKFontStyle.Bold);

    private static readonly Dictionary<int, SKMaskFilter> BlurCache = [];

    private readonly SKFont textFont = new(MonoTypeface);
    private readonly SKFont textFontBold = new(MonoBoldTypeface);
    private readonly SKPaint textPaint = new() { IsAntialias = true, Style = SKPaintStyle.Fill };

    private readonly Stopwatch clock = new();

    private CancellationTokenSource? cts;

    private float lastTime;

    protected SKPaint Stroke { get; } = new() { IsAntialias = true, Style = SKPaintStyle.Stroke };

    protected SKPaint Fill { get; } = new() { IsAntialias = true, Style = SKPaintStyle.Fill };

    protected AnimatedSkiaView()
    {
        BackgroundColor = Colors.Transparent;
        PaintSurface += OnPaintSurface;
    }

    protected override void OnHandlerChanged()
    {
        base.OnHandlerChanged();

        if (Handler is not null)
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

        clock.Start();
        cts = new CancellationTokenSource();
        _ = Loop(cts.Token);
    }

    private void StopTimer()
    {
        if (cts is null)
        {
            return;
        }

        clock.Stop();
        cts.Cancel();
        cts.Dispose();
        cts = null;
    }

    private async Task Loop(CancellationToken token)
    {
        try
        {
            using var timer = new PeriodicTimer(Interval);
            while (await timer.WaitForNextTickAsync(token))
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    if (token.IsCancellationRequested)
                    {
                        return;
                    }

                    var t = (float)clock.Elapsed.TotalSeconds;
                    var dt = Math.Clamp(t - lastTime, 0f, 0.1f);
                    lastTime = t;

                    Update(t, dt);
                    InvalidateSurface();
                });
            }
        }
        catch (OperationCanceledException)
        {
            // Ignore
        }
    }

    private void OnPaintSurface(object? sender, SKPaintSurfaceEventArgs e)
    {
        Render(e.Surface.Canvas, e.Info.Width, e.Info.Height, (float)clock.Elapsed.TotalSeconds);
    }

    protected abstract void Update(float t, float dt);

    protected abstract void Render(SKCanvas canvas, int width, int height, float t);

    //--------------------------------------------------------------------------------
    // Helpers
    //--------------------------------------------------------------------------------

    protected static float DegToRad(float degrees) => degrees * (MathF.PI / 180f);

    protected static bool Blink(float t, float hz) => ((t * hz) % 1f) < 0.55f;

    protected static SKMaskFilter GetBlur(float sigma)
    {
        var key = (int)(sigma * 10f);
        if (!BlurCache.TryGetValue(key, out var filter))
        {
            filter = SKMaskFilter.CreateBlur(SKBlurStyle.Normal, sigma);
            BlurCache[key] = filter;
        }

        return filter;
    }

    protected void DrawGlowLine(SKCanvas canvas, float x1, float y1, float x2, float y2, SKColor color, float width)
    {
        Stroke.StrokeCap = SKStrokeCap.Round;
        Stroke.Color = color.WithAlpha(55);
        Stroke.StrokeWidth = width + 4.5f;
        canvas.DrawLine(x1, y1, x2, y2, Stroke);
        Stroke.Color = color;
        Stroke.StrokeWidth = width;
        canvas.DrawLine(x1, y1, x2, y2, Stroke);
    }

    protected void DrawGlowPath(SKCanvas canvas, SKPath path, SKColor color, float width)
    {
        Stroke.StrokeCap = SKStrokeCap.Round;
        Stroke.Color = color.WithAlpha(55);
        Stroke.StrokeWidth = width + 4.5f;
        canvas.DrawPath(path, Stroke);
        Stroke.Color = color;
        Stroke.StrokeWidth = width;
        canvas.DrawPath(path, Stroke);
    }

    protected void DrawText(SKCanvas canvas, string text, float x, float y, float size, SKColor color, bool bold = false, SKTextAlign align = SKTextAlign.Left)
    {
        var font = bold ? textFontBold : textFont;
        font.Size = size;
        textPaint.Color = color;

        var tx = align switch
        {
            SKTextAlign.Center => x - (font.MeasureText(text) / 2f),
            SKTextAlign.Right => x - font.MeasureText(text),
            _ => x
        };
        canvas.DrawText(text, tx, y, SKTextAlign.Left, font, textPaint);
    }

    protected void DrawGlowText(SKCanvas canvas, string text, float x, float y, float size, SKColor color, float sigma, bool bold = false, SKTextAlign align = SKTextAlign.Left)
    {
        var font = bold ? textFontBold : textFont;
        font.Size = size;

        var tx = align switch
        {
            SKTextAlign.Center => x - (font.MeasureText(text) / 2f),
            SKTextAlign.Right => x - font.MeasureText(text),
            _ => x
        };

        textPaint.Color = color.WithAlpha(110);
        textPaint.MaskFilter = GetBlur(sigma);
        canvas.DrawText(text, tx, y, SKTextAlign.Left, font, textPaint);
        textPaint.MaskFilter = null;
        textPaint.Color = color;
        canvas.DrawText(text, tx, y, SKTextAlign.Left, font, textPaint);
    }

    protected float MeasureText(string text, float size, bool bold = false)
    {
        var font = bold ? textFontBold : textFont;
        font.Size = size;
        return font.MeasureText(text);
    }

    protected static SKPath CreateCutPanel(float x, float y, float w, float h, float cut)
    {
        using var builder = new SKPathBuilder();
        builder.MoveTo(x + cut, y);
        builder.LineTo(x + w - cut, y);
        builder.LineTo(x + w, y + cut);
        builder.LineTo(x + w, y + h - cut);
        builder.LineTo(x + w - cut, y + h);
        builder.LineTo(x + cut, y + h);
        builder.LineTo(x, y + h - cut);
        builder.LineTo(x, y + cut);
        builder.Close();
        return builder.Detach();
    }

    protected void DrawCutPanel(SKCanvas canvas, float x, float y, float w, float h, float cut, SKColor fill, SKColor border, float borderWidth)
    {
        using var path = CreateCutPanel(x, y, w, h, cut);
        Fill.Color = fill;
        canvas.DrawPath(path, Fill);
        Stroke.StrokeCap = SKStrokeCap.Butt;
        Stroke.Color = border;
        Stroke.StrokeWidth = borderWidth;
        canvas.DrawPath(path, Stroke);
    }
}
#pragma warning restore CA1001
