namespace Template.MobileApp.Controls;

using SkiaSharp.Views.Maui;

//--------------------------------------------------------------------------------
// Base
//--------------------------------------------------------------------------------
public abstract class HomeControl : SKCanvasView
{
    protected static readonly BindableProperty.BindingPropertyChangedDelegate Invalidate =
        static (bindable, _, _) => ((HomeControl)bindable).InvalidateSurface();

    // SKCanvas は物理ピクセルなので、論理単位の大きさ・太さにはこの密度を掛ける
    protected float GetDensity(SKImageInfo info) => Width > 0 ? (float)(info.Width / Width) : 1f;
}

//--------------------------------------------------------------------------------
// Background
//--------------------------------------------------------------------------------
// 写真(MauiImage のファイル名)を全面に敷き、ぼかして暗くする。ぼかしは写真か大きさが変わったときに 1 回だけ描き、以後は写す
public sealed partial class HomeBlurredImage : HomeControl
{
    // ぼかしは 4 分の 1 の大きさで描いて引き伸ばす(全面のぼかしをそのまま描くと重い)
    private const int Reduction = 4;

    public static readonly BindableProperty SourceProperty = BindableProperty.Create(
        nameof(Source),
        typeof(string),
        typeof(HomeBlurredImage),
        propertyChanged: Invalidate);

    public string? Source
    {
        get => (string?)GetValue(SourceProperty);
        set => SetValue(SourceProperty, value);
    }

    public static readonly BindableProperty BlurRadiusProperty = BindableProperty.Create(
        nameof(BlurRadius),
        typeof(double),
        typeof(HomeBlurredImage),
        16d,
        propertyChanged: Invalidate);

    public double BlurRadius
    {
        get => (double)GetValue(BlurRadiusProperty);
        set => SetValue(BlurRadiusProperty, value);
    }

    // 0〜1
    public static readonly BindableProperty DimProperty = BindableProperty.Create(
        nameof(Dim),
        typeof(double),
        typeof(HomeBlurredImage),
        0.5d,
        propertyChanged: Invalidate);

    public double Dim
    {
        get => (double)GetValue(DimProperty);
        set => SetValue(DimProperty, value);
    }

    private SKImage? cache;

    private string? cacheKey;

    protected override void OnHandlerChanging(HandlerChangingEventArgs args)
    {
        base.OnHandlerChanging(args);
        if (args.NewHandler is null)
        {
            cache?.Dispose();
            cache = null;
            cacheKey = null;
        }
    }

    protected override void OnPaintSurface(SKPaintSurfaceEventArgs e)
    {
        var canvas = e.Surface.Canvas;
        var info = e.Info;
        canvas.Clear(SKColors.Black);

        if ((info.Width > 0) && (info.Height > 0) && (Width > 0))
        {
            var key = $"{Source}:{info.Width}x{info.Height}:{BlurRadius}:{Dim}";
            if (key != cacheKey)
            {
                cache?.Dispose();
                cache = Render(info, GetDensity(info));
                cacheKey = key;
            }

            if (cache is not null)
            {
                canvas.DrawImage(cache, info.Rect, new SKSamplingOptions(SKFilterMode.Linear));
            }
        }
    }

    private SKImage? Render(SKImageInfo info, float density)
    {
        using var bitmap = LoadBitmap(Source);
        if (bitmap is null)
        {
            return null;
        }

        var width = Math.Max(1, info.Width / Reduction);
        var height = Math.Max(1, info.Height / Reduction);
        using var surface = SKSurface.Create(new SKImageInfo(width, height));
        var canvas = surface.Canvas;

        // 縦横の比を保って全面を覆う
        var scale = Math.Max(width / (float)bitmap.Width, height / (float)bitmap.Height);
        var dest = SKRect.Create((width - (bitmap.Width * scale)) / 2f, (height - (bitmap.Height * scale)) / 2f, bitmap.Width * scale, bitmap.Height * scale);
        var sigma = (float)BlurRadius * density / Reduction;
        using var paint = new SKPaint();
        paint.ImageFilter = SKImageFilter.CreateBlur(sigma, sigma, SKShaderTileMode.Clamp);
        using var image = SKImage.FromBitmap(bitmap);
        canvas.DrawImage(image, dest, new SKSamplingOptions(SKFilterMode.Linear), paint);
        canvas.DrawColor(SKColors.Black.WithAlpha((byte)(255 * Math.Clamp(Dim, 0d, 1d))), SKBlendMode.SrcOver);
        return surface.Snapshot();
    }

    private static partial SKBitmap? LoadBitmap(string? source);
}

//--------------------------------------------------------------------------------
// Thermostat
//--------------------------------------------------------------------------------
// 下を開けた 270° の円弧(左下が Minimum、右下が Maximum)。輪のあたりをドラッグすると Step 刻みで Value が変わる
public sealed class HomeThermostatDial : HomeControl
{
    private const float StartAngle = 135f;

    private const float SweepAngle = 270f;

    public static readonly BindableProperty ValueProperty = BindableProperty.Create(
        nameof(Value),
        typeof(double),
        typeof(HomeThermostatDial),
        0d,
        BindingMode.TwoWay,
        propertyChanged: Invalidate);

    public double Value
    {
        get => (double)GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    public static readonly BindableProperty MinimumProperty = BindableProperty.Create(
        nameof(Minimum),
        typeof(double),
        typeof(HomeThermostatDial),
        0d,
        propertyChanged: Invalidate);

    public double Minimum
    {
        get => (double)GetValue(MinimumProperty);
        set => SetValue(MinimumProperty, value);
    }

    public static readonly BindableProperty MaximumProperty = BindableProperty.Create(
        nameof(Maximum),
        typeof(double),
        typeof(HomeThermostatDial),
        1d,
        propertyChanged: Invalidate);

    public double Maximum
    {
        get => (double)GetValue(MaximumProperty);
        set => SetValue(MaximumProperty, value);
    }

    public static readonly BindableProperty StepProperty = BindableProperty.Create(
        nameof(Step),
        typeof(double),
        typeof(HomeThermostatDial),
        0d);

    public double Step
    {
        get => (double)GetValue(StepProperty);
        set => SetValue(StepProperty, value);
    }

    public static readonly BindableProperty ArcStartColorProperty = BindableProperty.Create(
        nameof(ArcStartColor),
        typeof(Color),
        typeof(HomeThermostatDial),
        Colors.Orange,
        propertyChanged: Invalidate);

    public Color ArcStartColor
    {
        get => (Color)GetValue(ArcStartColorProperty);
        set => SetValue(ArcStartColorProperty, value);
    }

    public static readonly BindableProperty ArcEndColorProperty = BindableProperty.Create(
        nameof(ArcEndColor),
        typeof(Color),
        typeof(HomeThermostatDial),
        Colors.OrangeRed,
        propertyChanged: Invalidate);

    public Color ArcEndColor
    {
        get => (Color)GetValue(ArcEndColorProperty);
        set => SetValue(ArcEndColorProperty, value);
    }

    public static readonly BindableProperty TrackColorProperty = BindableProperty.Create(
        nameof(TrackColor),
        typeof(Color),
        typeof(HomeThermostatDial),
        Color.FromRgba(255, 255, 255, 36),
        propertyChanged: Invalidate);

    public Color TrackColor
    {
        get => (Color)GetValue(TrackColorProperty);
        set => SetValue(TrackColorProperty, value);
    }

    public static readonly BindableProperty IsActiveProperty = BindableProperty.Create(
        nameof(IsActive),
        typeof(bool),
        typeof(HomeThermostatDial),
        true,
        propertyChanged: Invalidate);

    public bool IsActive
    {
        get => (bool)GetValue(IsActiveProperty);
        set => SetValue(IsActiveProperty, value);
    }

    private bool dragging;

    public HomeThermostatDial()
    {
        EnableTouchEvents = true;
        Touch += OnTouch;
    }

    private (SKPoint Center, float Radius, float Stroke, float Density) GetGeometry(SKImageInfo info)
    {
        var density = GetDensity(info);
        var stroke = 10f * density;
        // つまみと光が端で切れないように内側に描く
        var radius = (MathF.Min(info.Width, info.Height) / 2f) - (stroke * 2.4f);
        return (new SKPoint(info.Width / 2f, info.Height / 2f), radius, stroke, density);
    }

    private float Ratio()
    {
        var range = Maximum - Minimum;
        return range > 0 ? (float)Math.Clamp((Value - Minimum) / range, 0d, 1d) : 0f;
    }

    protected override void OnPaintSurface(SKPaintSurfaceEventArgs e)
    {
        var canvas = e.Surface.Canvas;
        var info = e.Info;
        canvas.Clear();

        if ((info.Width > 0) && (info.Height > 0))
        {
            var (center, radius, stroke, density) = GetGeometry(info);
            var rect = new SKRect(center.X - radius, center.Y - radius, center.X + radius, center.Y + radius);

            DrawDisc(canvas, center, radius - (stroke * 1.6f), density);

            using var track = new SKPaint();
            track.IsAntialias = true;
            track.Style = SKPaintStyle.Stroke;
            track.StrokeWidth = stroke;
            track.StrokeCap = SKStrokeCap.Round;
            track.Color = TrackColor.ToSKColor();
            using (var builder = new SKPathBuilder())
            {
                builder.AddArc(rect, StartAngle, SweepAngle);
                using var path = builder.Detach();
                canvas.DrawPath(path, track);
            }

            var ratio = Ratio();
            var start = IsActive ? ArcStartColor.ToSKColor() : SKColors.Gray;
            var end = IsActive ? ArcEndColor.ToSKColor() : SKColors.Gray;
            DrawValueArc(canvas, center, rect, stroke, density, ratio, start, end);
            DrawKnob(canvas, center, radius, stroke, density, ratio, end);
        }
    }

    // 内側の円(上が少し明るい半透明の面と細い縁)
    private static void DrawDisc(SKCanvas canvas, SKPoint center, float radius, float density)
    {
        using var shader = SKShader.CreateLinearGradient(
            new SKPoint(center.X, center.Y - radius),
            new SKPoint(center.X, center.Y + radius),
            [new SKColor(255, 255, 255, 40), new SKColor(255, 255, 255, 10)],
            [0f, 1f],
            SKShaderTileMode.Clamp);
        using var fill = new SKPaint();
        fill.IsAntialias = true;
        fill.Shader = shader;
        canvas.DrawCircle(center, radius, fill);

        using var line = new SKPaint();
        line.IsAntialias = true;
        line.Style = SKPaintStyle.Stroke;
        line.StrokeWidth = 1f * density;
        line.Color = new SKColor(255, 255, 255, 60);
        canvas.DrawCircle(center, radius, line);
    }

    private static void DrawValueArc(SKCanvas canvas, SKPoint center, SKRect rect, float stroke, float density, float ratio, SKColor start, SKColor end)
    {
        if (ratio > 0f)
        {
            var sweep = SweepAngle * ratio;
            // SKShader.CreateSweepGradient は 3 時の方向が 0° なので、円弧の始まりの角度まで回す
            using var baseShader = SKShader.CreateSweepGradient(
                center,
                [start, end],
                [0f, sweep / 360f],
                SKShaderTileMode.Clamp,
                0f,
                360f);
            using var shader = baseShader.WithLocalMatrix(SKMatrix.CreateRotationDegrees(StartAngle, center.X, center.Y));
            using var builder = new SKPathBuilder();
            builder.AddArc(rect, StartAngle, sweep);
            using var path = builder.Detach();

            using var glow = new SKPaint();
            glow.IsAntialias = true;
            glow.Style = SKPaintStyle.Stroke;
            glow.StrokeWidth = stroke * 2f;
            glow.StrokeCap = SKStrokeCap.Round;
            glow.Shader = shader;
            glow.Color = SKColors.White.WithAlpha(150);
            glow.MaskFilter = SKMaskFilter.CreateBlur(SKBlurStyle.Normal, 8f * density);
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

    private static void DrawKnob(SKCanvas canvas, SKPoint center, float radius, float stroke, float density, float ratio, SKColor color)
    {
        var angle = (StartAngle + (SweepAngle * ratio)) * MathF.PI / 180f;
        var knob = new SKPoint(center.X + (MathF.Cos(angle) * radius), center.Y + (MathF.Sin(angle) * radius));

        using var glow = new SKPaint();
        glow.IsAntialias = true;
        glow.Color = color.WithAlpha(200);
        glow.MaskFilter = SKMaskFilter.CreateBlur(SKBlurStyle.Normal, 6f * density);
        canvas.DrawCircle(knob, stroke * 1.2f, glow);

        using var ring = new SKPaint();
        ring.IsAntialias = true;
        ring.Color = SKColors.White;
        canvas.DrawCircle(knob, stroke * 0.95f, ring);

        using var dot = new SKPaint();
        dot.IsAntialias = true;
        dot.Color = color;
        canvas.DrawCircle(knob, stroke * 0.6f, dot);
    }

    //--------------------------------------------------------------------------------
    // Touch
    //--------------------------------------------------------------------------------

    private void OnTouch(object? sender, SKTouchEventArgs e)
    {
        switch (e.ActionType)
        {
            case SKTouchAction.Pressed:
                dragging = IsOnRing(e.Location);
                UpdateValue(e.Location);
                break;
            case SKTouchAction.Moved:
                UpdateValue(e.Location);
                break;
            case SKTouchAction.Released:
            case SKTouchAction.Cancelled:
            case SKTouchAction.Exited:
                dragging = false;
                break;
        }

        e.Handled = dragging;
    }

    // 内側の円(−・+ のボタンのある所)は受けない
    private bool IsOnRing(SKPoint location)
    {
        var (center, radius, stroke, _) = GetGeometry(new SKImageInfo(CanvasSize.Width > 0 ? (int)CanvasSize.Width : 1, CanvasSize.Height > 0 ? (int)CanvasSize.Height : 1));
        var distance = SKPoint.Distance(location, center);
        return (distance > radius - (stroke * 3f)) && (distance < radius + (stroke * 3f));
    }

    private void UpdateValue(SKPoint location)
    {
        if (dragging)
        {
            var center = new SKPoint(CanvasSize.Width / 2f, CanvasSize.Height / 2f);
            var angle = MathF.Atan2(location.Y - center.Y, location.X - center.X) * 180f / MathF.PI;
            var relative = (((angle - StartAngle) % 360f) + 360f) % 360f;
            // 下の開いた所は近い方の端にそろえる
            if (relative > SweepAngle)
            {
                relative = relative < SweepAngle + ((360f - SweepAngle) / 2f) ? SweepAngle : 0f;
            }

            var value = Minimum + ((Maximum - Minimum) * relative / SweepAngle);
            if (Step > 0)
            {
                value = Minimum + (Math.Round((value - Minimum) / Step) * Step);
            }

            Value = Math.Clamp(value, Minimum, Maximum);
        }
    }
}

//--------------------------------------------------------------------------------
// Icon
//--------------------------------------------------------------------------------
// MaterialIcons の文字のアイコン。IsGlowing の間は同じ色のぼかしを後ろに重ねて光らせる
public sealed class HomeGlowIcon : HomeControl
{
    public static readonly BindableProperty GlyphProperty = BindableProperty.Create(
        nameof(Glyph),
        typeof(string),
        typeof(HomeGlowIcon),
        string.Empty,
        propertyChanged: Invalidate);

    public string Glyph
    {
        get => (string)GetValue(GlyphProperty);
        set => SetValue(GlyphProperty, value);
    }

    public static readonly BindableProperty ColorProperty = BindableProperty.Create(
        nameof(Color),
        typeof(Color),
        typeof(HomeGlowIcon),
        Colors.White,
        propertyChanged: Invalidate);

    public Color Color
    {
        get => (Color)GetValue(ColorProperty);
        set => SetValue(ColorProperty, value);
    }

    public static readonly BindableProperty IconSizeProperty = BindableProperty.Create(
        nameof(IconSize),
        typeof(double),
        typeof(HomeGlowIcon),
        40d,
        propertyChanged: Invalidate);

    public double IconSize
    {
        get => (double)GetValue(IconSizeProperty);
        set => SetValue(IconSizeProperty, value);
    }

    public static readonly BindableProperty IsGlowingProperty = BindableProperty.Create(
        nameof(IsGlowing),
        typeof(bool),
        typeof(HomeGlowIcon),
        false,
        propertyChanged: Invalidate);

    public bool IsGlowing
    {
        get => (bool)GetValue(IsGlowingProperty);
        set => SetValue(IsGlowingProperty, value);
    }

    protected override void OnPaintSurface(SKPaintSurfaceEventArgs e)
    {
        var canvas = e.Surface.Canvas;
        var info = e.Info;
        canvas.Clear();

        if ((info.Width > 0) && (info.Height > 0) && (Width > 0) && !String.IsNullOrEmpty(Glyph))
        {
            var density = GetDensity(info);
            using var font = new SKFont(SocialFonts.MaterialIcons, (float)IconSize * density);
            var bounds = font.MeasureText(Glyph, out var rect);
            var x = (info.Width - bounds) / 2f;
            var y = (info.Height / 2f) - rect.MidY;
            var color = Color.ToSKColor();

            if (IsGlowing)
            {
                using var glow = new SKPaint();
                glow.IsAntialias = true;
                glow.Color = color.WithAlpha(200);
                glow.MaskFilter = SKMaskFilter.CreateBlur(SKBlurStyle.Normal, 9f * density);
                canvas.DrawText(Glyph, x, y, SKTextAlign.Left, font, glow);
            }

            using var paint = new SKPaint();
            paint.IsAntialias = true;
            paint.Color = color;
            canvas.DrawText(Glyph, x, y, SKTextAlign.Left, font, paint);
        }
    }
}

//--------------------------------------------------------------------------------
// Battery
//--------------------------------------------------------------------------------
// 横向きの電池(外形・右の端子・量のグラデーションと光)。Level は 0〜1
public sealed class HomeBatteryGauge : HomeControl
{
    public static readonly BindableProperty LevelProperty = BindableProperty.Create(
        nameof(Level),
        typeof(double),
        typeof(HomeBatteryGauge),
        0d,
        propertyChanged: Invalidate);

    public double Level
    {
        get => (double)GetValue(LevelProperty);
        set => SetValue(LevelProperty, value);
    }

    public static readonly BindableProperty FillColorProperty = BindableProperty.Create(
        nameof(FillColor),
        typeof(Color),
        typeof(HomeBatteryGauge),
        Colors.LimeGreen,
        propertyChanged: Invalidate);

    public Color FillColor
    {
        get => (Color)GetValue(FillColorProperty);
        set => SetValue(FillColorProperty, value);
    }

    protected override void OnPaintSurface(SKPaintSurfaceEventArgs e)
    {
        var canvas = e.Surface.Canvas;
        var info = e.Info;
        canvas.Clear();

        if ((info.Width > 0) && (info.Height > 0) && (Width > 0))
        {
            var density = GetDensity(info);
            var margin = 6f * density;
            var terminal = 4f * density;
            var body = new SKRect(margin, margin, info.Width - margin - terminal, info.Height - margin);
            var radius = 6f * density;
            var color = FillColor.ToSKColor();
            var level = (float)Math.Clamp(Level, 0d, 1d);

            var inset = 3f * density;
            var inner = new SKRect(body.Left + inset, body.Top + inset, body.Right - inset, body.Bottom - inset);
            if (level > 0f)
            {
                var fill = new SKRect(inner.Left, inner.Top, inner.Left + (inner.Width * level), inner.Bottom);

                using var glow = new SKPaint();
                glow.IsAntialias = true;
                glow.Color = color.WithAlpha(150);
                glow.MaskFilter = SKMaskFilter.CreateBlur(SKBlurStyle.Normal, 5f * density);
                canvas.DrawRoundRect(fill, radius / 2f, radius / 2f, glow);

                using var shader = SKShader.CreateLinearGradient(
                    new SKPoint(0f, fill.Top),
                    new SKPoint(0f, fill.Bottom),
                    [color.WithAlpha(255), color.WithAlpha(170)],
                    [0f, 1f],
                    SKShaderTileMode.Clamp);
                using var paint = new SKPaint();
                paint.IsAntialias = true;
                paint.Shader = shader;
                canvas.DrawRoundRect(fill, radius / 2f, radius / 2f, paint);
            }

            using var line = new SKPaint();
            line.IsAntialias = true;
            line.Style = SKPaintStyle.Stroke;
            line.StrokeWidth = 2f * density;
            line.Color = color.WithAlpha(220);
            canvas.DrawRoundRect(body, radius, radius, line);

            using var cap = new SKPaint();
            cap.IsAntialias = true;
            cap.Color = color.WithAlpha(220);
            var capHeight = body.Height * 0.4f;
            canvas.DrawRoundRect(new SKRect(body.Right + density, body.MidY - (capHeight / 2f), body.Right + terminal, body.MidY + (capHeight / 2f)), density, density, cap);
        }
    }
}
