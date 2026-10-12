namespace Template.MobileApp.Controls;

using SkiaSharp;
using SkiaSharp.Views.Maui;
using SkiaSharp.Views.Maui.Controls;

public sealed class WeatherMoonPhaseView : SKCanvasView
{
    private const double SynodicMonth = 29.530588853;

    private const float MoonMargin = 8;

    private const float GlowMinimumAlpha = 30;

    private const float GlowAlphaRange = 70;

    private const float GlowBlur = 6;

    private const float GlowSpread = 2;

    private const float HighlightOffsetRate = 0.3f;

    private const float HighlightRadiusRate = 1.6f;

    private const byte LitEdgeAlpha = 210;

    private const byte SeaAlpha = 22;

    private static readonly (float X, float Y, float Size)[] Seas = [(-0.28f, -0.22f, 0.22f), (0.25f, 0.05f, 0.28f), (-0.05f, 0.42f, 0.16f)];

    //--------------------------------------------------------------------------------
    // Property
    //--------------------------------------------------------------------------------

    public static readonly BindableProperty AgeProperty = BindableProperty.Create(
        nameof(Age),
        typeof(double),
        typeof(WeatherMoonPhaseView),
        0d,
        propertyChanged: OnVisualChanged);

    public static readonly BindableProperty LightColorProperty = BindableProperty.Create(
        nameof(LightColor),
        typeof(Color),
        typeof(WeatherMoonPhaseView),
        Color.FromArgb("#FFF4C7"),
        propertyChanged: OnVisualChanged);

    public static readonly BindableProperty DarkColorProperty = BindableProperty.Create(
        nameof(DarkColor),
        typeof(Color),
        typeof(WeatherMoonPhaseView),
        Color.FromRgba(255, 255, 255, 36),
        propertyChanged: OnVisualChanged);

    public double Age
    {
        get => (double)GetValue(AgeProperty);
        set => SetValue(AgeProperty, value);
    }

    public Color LightColor
    {
        get => (Color)GetValue(LightColorProperty);
        set => SetValue(LightColorProperty, value);
    }

    public Color DarkColor
    {
        get => (Color)GetValue(DarkColorProperty);
        set => SetValue(DarkColorProperty, value);
    }

    //--------------------------------------------------------------------------------
    // Render
    //--------------------------------------------------------------------------------

    private static void OnVisualChanged(BindableObject bindable, object oldValue, object newValue) =>
        ((WeatherMoonPhaseView)bindable).InvalidateSurface();

    protected override void OnPaintSurface(SKPaintSurfaceEventArgs e)
    {
        var canvas = e.Surface.Canvas;
        var info = e.Info;
        canvas.Clear();

        if ((info.Width > 0) && (info.Height > 0) && (Width > 0))
        {
            var density = (float)(info.Width / Width);
            var center = new SKPoint(info.Width / 2f, info.Height / 2f);
            var radius = (MathF.Min(info.Width, info.Height) / 2f) - (MoonMargin * density);
            var phase = (float)((((Age % SynodicMonth) + SynodicMonth) % SynodicMonth) / SynodicMonth);
            var light = LightColor.ToSKColor();

            using var glow = new SKPaint();
            glow.IsAntialias = true;
            glow.Color = light.WithAlpha((byte)(GlowMinimumAlpha + (GlowAlphaRange * (1 - MathF.Cos(phase * 2 * MathF.PI)) / 2)));
            glow.MaskFilter = SKMaskFilter.CreateBlur(SKBlurStyle.Normal, GlowBlur * density);
            canvas.DrawCircle(center, radius + (GlowSpread * density), glow);

            using var paint = new SKPaint();
            paint.IsAntialias = true;
            paint.Color = DarkColor.ToSKColor();
            canvas.DrawCircle(center, radius, paint);

            using var path = CreateLitPath(center, radius, phase);
            using var shader = SKShader.CreateRadialGradient(
                new SKPoint(center.X - (radius * HighlightOffsetRate), center.Y - (radius * HighlightOffsetRate)),
                radius * HighlightRadiusRate,
                [light, light.WithAlpha(LitEdgeAlpha)],
                SKShaderTileMode.Clamp);
            paint.Shader = shader;
            canvas.DrawPath(path, paint);
            paint.Shader = null;

            canvas.Save();
            canvas.ClipPath(path, antialias: true);
            paint.Color = SKColors.Black.WithAlpha(SeaAlpha);
            foreach (var (x, y, size) in Seas)
            {
                canvas.DrawCircle(center.X + (radius * x), center.Y + (radius * y), radius * size, paint);
            }

            canvas.Restore();
        }
    }

    private static SKPath CreateLitPath(SKPoint center, float radius, float phase)
    {
        var circle = new SKRect(center.X - radius, center.Y - radius, center.X + radius, center.Y + radius);
        var half = radius * MathF.Abs(MathF.Cos(phase * 2 * MathF.PI));
        var terminator = new SKRect(center.X - half, center.Y - radius, center.X + half, center.Y + radius);
        var waxing = phase < 0.5f;
        var crescent = (phase < 0.25f) || (phase >= 0.75f);

        using var builder = new SKPathBuilder();
        builder.MoveTo(center.X, center.Y - radius);
        builder.ArcTo(circle, -90, waxing ? 180 : -180, false);

        var towardRight = waxing == crescent;
        builder.ArcTo(terminator, 90, towardRight ? -180 : 180, false);
        builder.Close();
        return builder.Detach();
    }
}
