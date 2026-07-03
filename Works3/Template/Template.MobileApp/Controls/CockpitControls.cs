namespace Template.MobileApp.Controls;

using SkiaSharp;
using SkiaSharp.Views.Maui;
using SkiaSharp.Views.Maui.Controls;

//--------------------------------------------------------------------------------
// Attitude indicator
//--------------------------------------------------------------------------------

// 姿勢指示器(水平儀)。Pitch/Roll に応じて空と大地の 2 色盤を回転・平行移動する
public sealed class AttitudeIndicator : SKCanvasView
{
    private const float PixelPerPitchDeg = 4.5f;

    private static readonly SKColor SkyColor = new(0x2E, 0x6D, 0xB4);
    private static readonly SKColor SkyLightColor = new(0x4A, 0x90, 0xD9);
    private static readonly SKColor GroundColor = new(0x6B, 0x46, 0x26);
    private static readonly SKColor GroundLightColor = new(0x8B, 0x5E, 0x34);
    private static readonly SKColor LineColor = SKColors.White;
    private static readonly SKColor MarkColor = new(0xFF, 0xD5, 0x4F);
    private static readonly SKColor BezelColor = new(0x22, 0x28, 0x2E);

    public static readonly BindableProperty PitchProperty = BindableProperty.Create(
        nameof(Pitch),
        typeof(float),
        typeof(AttitudeIndicator),
        0f,
        propertyChanged: Invalidate);

    public float Pitch
    {
        get => (float)GetValue(PitchProperty);
        set => SetValue(PitchProperty, value);
    }

    public static readonly BindableProperty RollProperty = BindableProperty.Create(
        nameof(Roll),
        typeof(float),
        typeof(AttitudeIndicator),
        0f,
        propertyChanged: Invalidate);

    public float Roll
    {
        get => (float)GetValue(RollProperty);
        set => SetValue(RollProperty, value);
    }

    public AttitudeIndicator()
    {
        BackgroundColor = Colors.Transparent;
    }

    private static void Invalidate(BindableObject bindable, object oldValue, object newValue)
    {
        ((AttitudeIndicator)bindable).InvalidateSurface();
    }

    protected override void OnPaintSurface(SKPaintSurfaceEventArgs e)
    {
        base.OnPaintSurface(e);

        var canvas = e.Surface.Canvas;
        canvas.Clear(SKColors.Transparent);

        var scale = (float)(e.Info.Width / Width);
        if (!float.IsFinite(scale) || (scale <= 0))
        {
            return;
        }

        canvas.Scale(scale);

        var width = (float)Width;
        var height = (float)Height;
        var cx = width / 2f;
        var cy = height / 2f;
        var radius = (MathF.Min(width, height) / 2f) - 10f;

        using var paint = new SKPaint();
        paint.IsAntialias = true;

        // 空と大地(クリップ円内で回転+平行移動)
        canvas.Save();
        using (var builder = new SKPathBuilder())
        {
            builder.AddCircle(cx, cy, radius);
            using var clip = builder.Detach();
            canvas.ClipPath(clip, antialias: true);
        }

        canvas.RotateDegrees(-Roll, cx, cy);
        var horizonY = cy + (Pitch * PixelPerPitchDeg);
        var extent = radius * 2.5f;

        paint.Style = SKPaintStyle.Fill;
        using (var sky = SKShader.CreateLinearGradient(
                   new SKPoint(cx, horizonY - extent),
                   new SKPoint(cx, horizonY),
                   [SkyLightColor, SkyColor],
                   SKShaderTileMode.Clamp))
        {
            paint.Shader = sky;
            canvas.DrawRect(cx - extent, horizonY - extent, extent * 2, extent, paint);
        }
        using (var ground = SKShader.CreateLinearGradient(
                   new SKPoint(cx, horizonY),
                   new SKPoint(cx, horizonY + extent),
                   [GroundColor, GroundLightColor],
                   SKShaderTileMode.Clamp))
        {
            paint.Shader = ground;
            canvas.DrawRect(cx - extent, horizonY, extent * 2, extent, paint);
        }
        paint.Shader = null;

        // 水平線
        paint.Style = SKPaintStyle.Stroke;
        paint.StrokeWidth = 2.5f;
        paint.Color = LineColor;
        canvas.DrawLine(cx - extent, horizonY, cx + extent, horizonY, paint);

        // ピッチラダー
        using var font = new SKFont(SKTypeface.Default, size: 12f);
        for (var p = -20; p <= 20; p += 5)
        {
            if (p == 0)
            {
                continue;
            }

            var y = horizonY - (p * PixelPerPitchDeg);
            var w = (p % 10) == 0 ? 36f : 20f;
            paint.StrokeWidth = 1.6f;
            canvas.DrawLine(cx - w, y, cx + w, y, paint);

            if ((p % 10) == 0)
            {
                paint.Style = SKPaintStyle.Fill;
                canvas.DrawText($"{Math.Abs(p)}", cx - w - 8f, y + 4f, SKTextAlign.Right, font, paint);
                canvas.DrawText($"{Math.Abs(p)}", cx + w + 8f, y + 4f, SKTextAlign.Left, font, paint);
                paint.Style = SKPaintStyle.Stroke;
            }
        }

        canvas.Restore();

        // ロール目盛(上部の弧)
        paint.Style = SKPaintStyle.Stroke;
        paint.Color = LineColor;
        for (var a = -60; a <= 60; a += 15)
        {
            var rad = (a - 90) * MathF.PI / 180f;
            var len = a == 0 ? 12f : (a % 30) == 0 ? 9f : 6f;
            var x1 = cx + (radius * MathF.Cos(rad));
            var y1 = cy + (radius * MathF.Sin(rad));
            var x2 = cx + ((radius - len) * MathF.Cos(rad));
            var y2 = cy + ((radius - len) * MathF.Sin(rad));
            paint.StrokeWidth = a == 0 ? 2.2f : 1.5f;
            canvas.DrawLine(x1, y1, x2, y2, paint);
        }

        // ロールポインタ(バンク角)
        var rollRad = (-Roll - 90) * MathF.PI / 180f;
        var px = cx + ((radius - 14f) * MathF.Cos(rollRad));
        var py = cy + ((radius - 14f) * MathF.Sin(rollRad));
        paint.Style = SKPaintStyle.Fill;
        paint.Color = MarkColor;
        using (var builder = new SKPathBuilder())
        {
            builder.MoveTo(px, py - 6f);
            builder.LineTo(px - 6f, py + 6f);
            builder.LineTo(px + 6f, py + 6f);
            builder.Close();
            using var tri = builder.Detach();
            canvas.DrawPath(tri, paint);
        }

        // 固定機体マーク
        paint.Style = SKPaintStyle.Stroke;
        paint.StrokeWidth = 4f;
        paint.StrokeCap = SKStrokeCap.Round;
        paint.Color = MarkColor;
        canvas.DrawLine(cx - 56f, cy, cx - 20f, cy, paint);
        canvas.DrawLine(cx - 20f, cy, cx - 10f, cy + 10f, paint);
        canvas.DrawLine(cx + 56f, cy, cx + 20f, cy, paint);
        canvas.DrawLine(cx + 20f, cy, cx + 10f, cy + 10f, paint);
        paint.Style = SKPaintStyle.Fill;
        canvas.DrawCircle(cx, cy, 3.5f, paint);

        // ベゼル
        paint.Style = SKPaintStyle.Stroke;
        paint.StrokeWidth = 6f;
        paint.Color = BezelColor;
        canvas.DrawCircle(cx, cy, radius + 3f, paint);
    }
}

//--------------------------------------------------------------------------------
// Compass ribbon
//--------------------------------------------------------------------------------

// 横スクロールする方位リボン
public sealed class CompassRibbon : SKCanvasView
{
    private const float PixelPerDeg = 3f;

    private static readonly SKColor BgColor = new(0x10, 0x14, 0x18);
    private static readonly SKColor TickColor = new(0xA5, 0xD6, 0xA7);
    private static readonly SKColor CardinalColor = new(0xFF, 0xD5, 0x4F);
    private static readonly SKColor MarkerColor = new(0xFF, 0xB3, 0x00);

    public static readonly BindableProperty HeadingProperty = BindableProperty.Create(
        nameof(Heading),
        typeof(float),
        typeof(CompassRibbon),
        0f,
        propertyChanged: Invalidate);

    public float Heading
    {
        get => (float)GetValue(HeadingProperty);
        set => SetValue(HeadingProperty, value);
    }

    public CompassRibbon()
    {
        BackgroundColor = Colors.Transparent;
        HeightRequest = 56;
    }

    private static void Invalidate(BindableObject bindable, object oldValue, object newValue)
    {
        ((CompassRibbon)bindable).InvalidateSurface();
    }

    protected override void OnPaintSurface(SKPaintSurfaceEventArgs e)
    {
        base.OnPaintSurface(e);

        var canvas = e.Surface.Canvas;
        canvas.Clear(SKColors.Transparent);

        var scale = (float)(e.Info.Width / Width);
        if (!float.IsFinite(scale) || (scale <= 0))
        {
            return;
        }

        canvas.Scale(scale);

        var width = (float)Width;
        var height = (float)Height;
        var cx = width / 2f;
        var heading = ((Heading % 360f) + 360f) % 360f;

        using var paint = new SKPaint();
        paint.IsAntialias = true;
        paint.Style = SKPaintStyle.Fill;
        paint.Color = BgColor;
        canvas.DrawRoundRect(new SKRect(0, 0, width, height), 8f, 8f, paint);

        using var font = new SKFont(SKTypeface.Default, size: 13f);
        using var boldFont = new SKFont(SKTypeface.FromFamilyName(null, SKFontStyle.Bold), size: 14f);

        var start = (MathF.Floor(heading / 5f) * 5f) - 80f;
        for (var a = start; a <= start + 165f; a += 5f)
        {
            var x = cx + ((a - heading) * PixelPerDeg);
            if ((x < 12f) || (x > width - 12f))
            {
                continue;
            }

            var value = (int)(((a % 360f) + 360f) % 360f);
            var major = (value % 30) == 0;

            paint.Style = SKPaintStyle.Stroke;
            paint.StrokeWidth = major ? 2f : 1.2f;
            paint.Color = TickColor.WithAlpha(major ? (byte)230 : (byte)130);
            canvas.DrawLine(x, height - 14f, x, height - (major ? 26f : 20f), paint);

            if (major)
            {
                paint.Style = SKPaintStyle.Fill;
                var text = value switch
                {
                    0 => "N",
                    90 => "E",
                    180 => "S",
                    270 => "W",
                    _ => $"{value / 10:00}",
                };
                var cardinal = value is 0 or 90 or 180 or 270;
                paint.Color = cardinal ? CardinalColor : TickColor;
                canvas.DrawText(text, x, height - 32f, SKTextAlign.Center, cardinal ? boldFont : font, paint);
            }
        }

        // 中央マーカーと現在値
        paint.Style = SKPaintStyle.Fill;
        paint.Color = MarkerColor;
        using (var builder = new SKPathBuilder())
        {
            builder.MoveTo(cx, height - 8f);
            builder.LineTo(cx - 6f, height - 2f);
            builder.LineTo(cx + 6f, height - 2f);
            builder.Close();
            using var tri = builder.Detach();
            canvas.DrawPath(tri, paint);
        }

        canvas.DrawText($"{(int)heading:000}°", cx, 14f, SKTextAlign.Center, boldFont, paint);
    }
}
