namespace Template.MobileApp.Controls;

using SkiaSharp;
using SkiaSharp.Views.Maui;
using SkiaSharp.Views.Maui.Controls;

// Progress は 0〜1 で 12 時から時計回り
public sealed class GlowRing : SKCanvasView
{
    public static readonly BindableProperty ProgressProperty = BindableProperty.Create(
        nameof(Progress),
        typeof(double),
        typeof(GlowRing),
        0d,
        propertyChanged: OnVisualChanged);

    public double Progress
    {
        get => (double)GetValue(ProgressProperty);
        set => SetValue(ProgressProperty, value);
    }

    public static readonly BindableProperty StartColorProperty = BindableProperty.Create(
        nameof(StartColor),
        typeof(Color),
        typeof(GlowRing),
        Colors.Cyan,
        propertyChanged: OnVisualChanged);

    public Color StartColor
    {
        get => (Color)GetValue(StartColorProperty);
        set => SetValue(StartColorProperty, value);
    }

    public static readonly BindableProperty EndColorProperty = BindableProperty.Create(
        nameof(EndColor),
        typeof(Color),
        typeof(GlowRing),
        Colors.DodgerBlue,
        propertyChanged: OnVisualChanged);

    public Color EndColor
    {
        get => (Color)GetValue(EndColorProperty);
        set => SetValue(EndColorProperty, value);
    }

    public static readonly BindableProperty TrackColorProperty = BindableProperty.Create(
        nameof(TrackColor),
        typeof(Color),
        typeof(GlowRing),
        Color.FromRgba(255, 255, 255, 20),
        propertyChanged: OnVisualChanged);

    public Color TrackColor
    {
        get => (Color)GetValue(TrackColorProperty);
        set => SetValue(TrackColorProperty, value);
    }

    public static readonly BindableProperty ThicknessProperty = BindableProperty.Create(
        nameof(Thickness),
        typeof(double),
        typeof(GlowRing),
        8d,
        propertyChanged: OnVisualChanged);

    public double Thickness
    {
        get => (double)GetValue(ThicknessProperty);
        set => SetValue(ThicknessProperty, value);
    }

    private static void OnVisualChanged(BindableObject bindable, object oldValue, object newValue) =>
        ((GlowRing)bindable).InvalidateSurface();

    protected override void OnPaintSurface(SKPaintSurfaceEventArgs e)
    {
        var canvas = e.Surface.Canvas;
        var info = e.Info;
        canvas.Clear();

        if ((info.Width > 0) && (info.Height > 0) && (Width > 0))
        {
            // SKCanvas は物理ピクセルなので、論理単位の太さに密度を掛ける
            var density = (float)(info.Width / Width);
            var stroke = (float)Thickness * density;
            var blur = stroke * 0.6f;
            // 光の太さの半分と、ぼかしの半径の 2 倍だけ内側に描いて、光がビューの端で切れないようにする
            var radius = (MathF.Min(info.Width, info.Height) / 2f) - (stroke * 0.8f) - (blur * 2f);
            var center = new SKPoint(info.Width / 2f, info.Height / 2f);

            using var track = new SKPaint();
            track.IsAntialias = true;
            track.Style = SKPaintStyle.Stroke;
            track.StrokeWidth = stroke;
            track.Color = TrackColor.ToSKColor();
            canvas.DrawCircle(center, radius, track);

            DrawGlowArc(canvas, center, radius, stroke, blur, (float)Math.Clamp(Progress, 0d, 1d), StartColor.ToSKColor(), EndColor.ToSKColor());
        }
    }

    // TimerDial と共通
    internal static void DrawGlowArc(SKCanvas canvas, SKPoint center, float radius, float stroke, float blur, float progress, SKColor startColor, SKColor endColor)
    {
        if (progress > 0f)
        {
            DrawArc(canvas, center, radius, stroke, blur, progress, startColor, endColor);
        }
    }

    private static void DrawArc(SKCanvas canvas, SKPoint center, float radius, float stroke, float blur, float progress, SKColor startColor, SKColor endColor)
    {
        var sweep = 360f * progress;
        var rect = new SKRect(center.X - radius, center.Y - radius, center.X + radius, center.Y + radius);

        // SKShader.CreateSweepGradient は 3 時の方向が 0° なので、12 時から始まるように -90° 回す
        using var baseShader = SKShader.CreateSweepGradient(
            center,
            [startColor, endColor],
            [0f, progress],
            SKShaderTileMode.Clamp,
            0f,
            360f);
        using var shader = baseShader.WithLocalMatrix(SKMatrix.CreateRotationDegrees(-90f, center.X, center.Y));
        using var builder = new SKPathBuilder();
        builder.AddArc(rect, -90f, sweep);
        using var path = builder.Detach();

        using var glow = new SKPaint();
        glow.IsAntialias = true;
        glow.Style = SKPaintStyle.Stroke;
        glow.StrokeWidth = stroke * 1.8f;
        glow.StrokeCap = SKStrokeCap.Round;
        glow.Shader = shader;
        glow.Color = SKColors.White.WithAlpha(140);
        glow.MaskFilter = SKMaskFilter.CreateBlur(SKBlurStyle.Normal, blur);
        canvas.DrawPath(path, glow);

        using var paint = new SKPaint();
        paint.IsAntialias = true;
        paint.Style = SKPaintStyle.Stroke;
        paint.StrokeWidth = stroke;
        paint.StrokeCap = SKStrokeCap.Round;
        paint.Shader = shader;
        canvas.DrawPath(path, paint);
    }
}
