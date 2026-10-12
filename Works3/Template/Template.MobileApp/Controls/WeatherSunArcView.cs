namespace Template.MobileApp.Controls;

using SkiaSharp;
using SkiaSharp.Views.Maui;
using SkiaSharp.Views.Maui.Controls;

public sealed class WeatherSunArcView : SKCanvasView
{
    private const string RevealAnimationName = "SunArcReveal";

    private const uint AnimationRate = 16;

    private const uint RevealLength = 900;

    private const float SunRadius = 7f;

    private const float SunMargin = 6f;

    private const float PathStrokeWidth = 1.5f;

    private const float PathDash = 4f;

    private const float PassedStrokeWidth = 3f;

    private const float HorizonStrokeWidth = 1f;

    private const float GlowBlur = 5f;

    private const float GlowRadiusRate = 1.4f;

    private const byte FillAlpha = 110;

    private const byte DayGlowAlpha = 150;

    private const byte NightGlowAlpha = 60;

    private const byte NightSunAlpha = 110;

    //--------------------------------------------------------------------------------
    // Member
    //--------------------------------------------------------------------------------

    private float reveal = 1f;

    //--------------------------------------------------------------------------------
    // Property
    //--------------------------------------------------------------------------------

    public static readonly BindableProperty SunriseProperty = BindableProperty.Create(
        nameof(Sunrise),
        typeof(DateTime),
        typeof(WeatherSunArcView),
        default(DateTime),
        propertyChanged: OnVisualChanged);

    public static readonly BindableProperty SunsetProperty = BindableProperty.Create(
        nameof(Sunset),
        typeof(DateTime),
        typeof(WeatherSunArcView),
        default(DateTime),
        propertyChanged: OnVisualChanged);

    public static readonly BindableProperty NowProperty = BindableProperty.Create(
        nameof(Now),
        typeof(DateTime),
        typeof(WeatherSunArcView),
        default(DateTime),
        propertyChanged: OnVisualChanged);

    public static readonly BindableProperty PathColorProperty = BindableProperty.Create(
        nameof(PathColor),
        typeof(Color),
        typeof(WeatherSunArcView),
        Color.FromRgba(255, 255, 255, 90),
        propertyChanged: OnVisualChanged);

    public static readonly BindableProperty SunColorProperty = BindableProperty.Create(
        nameof(SunColor),
        typeof(Color),
        typeof(WeatherSunArcView),
        Color.FromArgb("#FFD54F"),
        propertyChanged: OnVisualChanged);

    public DateTime Sunrise
    {
        get => (DateTime)GetValue(SunriseProperty);
        set => SetValue(SunriseProperty, value);
    }

    public DateTime Sunset
    {
        get => (DateTime)GetValue(SunsetProperty);
        set => SetValue(SunsetProperty, value);
    }

    public DateTime Now
    {
        get => (DateTime)GetValue(NowProperty);
        set => SetValue(NowProperty, value);
    }

    public Color PathColor
    {
        get => (Color)GetValue(PathColorProperty);
        set => SetValue(PathColorProperty, value);
    }

    public Color SunColor
    {
        get => (Color)GetValue(SunColorProperty);
        set => SetValue(SunColorProperty, value);
    }

    //--------------------------------------------------------------------------------
    // Constructor
    //--------------------------------------------------------------------------------

    public WeatherSunArcView()
    {
        Loaded += (_, _) => StartReveal();
        Unloaded += (_, _) => this.AbortAnimation(RevealAnimationName);
    }

    //--------------------------------------------------------------------------------
    // Render
    //--------------------------------------------------------------------------------

    private static void OnVisualChanged(BindableObject bindable, object oldValue, object newValue) =>
        ((WeatherSunArcView)bindable).InvalidateSurface();

    protected override void OnPaintSurface(SKPaintSurfaceEventArgs e)
    {
        var canvas = e.Surface.Canvas;
        var info = e.Info;
        canvas.Clear();

        if ((info.Width > 0) && (info.Height > 0) && (Width > 0) && (Sunset > Sunrise))
        {
            var density = (float)(info.Width / Width);
            var sun = SunRadius * density;
            var margin = sun + (SunMargin * density);
            var horizon = info.Height - margin;

            var oval = new SKRect(margin, margin, info.Width - margin, (horizon * 2) - margin);
            var progress = (float)Math.Clamp((Now - Sunrise).TotalMinutes / (Sunset - Sunrise).TotalMinutes, 0, 1) * reveal;
            var isDay = (Now > Sunrise) && (Now < Sunset);
            var sunColor = SunColor.ToSKColor();

            using var paint = new SKPaint();
            paint.IsAntialias = true;
            paint.Style = SKPaintStyle.Stroke;
            paint.StrokeWidth = PathStrokeWidth * density;
            paint.Color = PathColor.ToSKColor();
            paint.PathEffect = SKPathEffect.CreateDash([PathDash * density, PathDash * density], 0);
            DrawArc(canvas, oval, 180, 180, paint);
            paint.PathEffect = null;

            if (progress > 0)
            {
                using var fill = new SKPaint();
                fill.IsAntialias = true;
                using var shader = SKShader.CreateLinearGradient(
                    new SKPoint(0, margin),
                    new SKPoint(0, horizon),
                    [sunColor.WithAlpha(FillAlpha), sunColor.WithAlpha(0)],
                    SKShaderTileMode.Clamp);
                fill.Shader = shader;
                using var builder = new SKPathBuilder();
                builder.AddArc(oval, 180, 180 * progress);
                var end = PointAt(oval, 180 + (180 * progress));
                builder.LineTo(end.X, horizon);
                builder.LineTo(oval.Left, horizon);
                builder.Close();
                using var path = builder.Detach();
                canvas.DrawPath(path, fill);

                paint.StrokeWidth = PassedStrokeWidth * density;
                paint.StrokeCap = SKStrokeCap.Round;
                paint.Color = sunColor;
                DrawArc(canvas, oval, 180, 180 * progress, paint);
            }

            paint.StrokeWidth = HorizonStrokeWidth * density;
            paint.Color = PathColor.ToSKColor();
            canvas.DrawLine(0, horizon, info.Width, horizon, paint);

            var position = PointAt(oval, 180 + (180 * progress));
            using var glow = new SKPaint();
            glow.IsAntialias = true;
            glow.Color = sunColor.WithAlpha(isDay ? DayGlowAlpha : NightGlowAlpha);
            glow.MaskFilter = SKMaskFilter.CreateBlur(SKBlurStyle.Normal, GlowBlur * density);
            canvas.DrawCircle(position, sun * GlowRadiusRate, glow);

            paint.Style = SKPaintStyle.Fill;
            paint.Color = isDay ? sunColor : sunColor.WithAlpha(NightSunAlpha);
            canvas.DrawCircle(position, sun, paint);
        }
    }

    private static SKPoint PointAt(SKRect oval, float angle)
    {
        var radian = angle * MathF.PI / 180f;
        return new SKPoint(oval.MidX + (MathF.Cos(radian) * oval.Width / 2), oval.MidY + (MathF.Sin(radian) * oval.Height / 2));
    }

    private static void DrawArc(SKCanvas canvas, SKRect oval, float start, float sweep, SKPaint paint)
    {
        using var builder = new SKPathBuilder();
        builder.AddArc(oval, start, sweep);
        using var path = builder.Detach();
        canvas.DrawPath(path, paint);
    }

    //--------------------------------------------------------------------------------
    // Animation
    //--------------------------------------------------------------------------------

    private void StartReveal()
    {
        this.AbortAnimation(RevealAnimationName);
        reveal = 0f;
        this.Animate(
            RevealAnimationName,
            v =>
            {
                reveal = (float)v;
                InvalidateSurface();
            },
            0d,
            1d,
            AnimationRate,
            RevealLength,
            Easing.CubicOut);
    }
}
