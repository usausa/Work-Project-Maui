namespace Template.MobileApp.Controls;

using SkiaSharp;
using SkiaSharp.Views.Maui;
using SkiaSharp.Views.Maui.Controls;

public sealed class WeatherCompassDial : SKCanvasView
{
    private const float DialMargin = 4f;

    private const float LabelInset = 20f;

    private const float ArrowInset = 12f;

    private const int TickStep = 5;

    private const int MajorTickStep = 30;

    private const float MajorTickWidth = 2f;

    private const float MinorTickWidth = 1f;

    private const float MajorTickLength = 7f;

    private const float MinorTickLength = 4f;

    private const float ArrowGapRate = 0.45f;

    private const float ArrowStrokeWidth = 2.5f;

    private const float TailInset = 4f;

    private const float TailRadius = 3.5f;

    private const float ShaftInset = 8f;

    private const float HeadLength = 12f;

    private const float HeadHalfWidth = 6f;

    private static readonly SKTypeface BoldTypeface = SKTypeface.FromFamilyName("sans-serif", SKFontStyle.Bold);

    private static readonly string[] Labels = ["N", "E", "S", "W"];

    //--------------------------------------------------------------------------------
    // Property
    //--------------------------------------------------------------------------------

    public static readonly BindableProperty DirectionProperty = BindableProperty.Create(
        nameof(Direction),
        typeof(double),
        typeof(WeatherCompassDial),
        0d,
        propertyChanged: OnVisualChanged);

    public static readonly BindableProperty TickColorProperty = BindableProperty.Create(
        nameof(TickColor),
        typeof(Color),
        typeof(WeatherCompassDial),
        Color.FromRgba(255, 255, 255, 110),
        propertyChanged: OnVisualChanged);

    public static readonly BindableProperty LabelColorProperty = BindableProperty.Create(
        nameof(LabelColor),
        typeof(Color),
        typeof(WeatherCompassDial),
        Colors.White,
        propertyChanged: OnVisualChanged);

    public static readonly BindableProperty LabelFontSizeProperty = BindableProperty.Create(
        nameof(LabelFontSize),
        typeof(double),
        typeof(WeatherCompassDial),
        12d,
        propertyChanged: OnVisualChanged);

    public static readonly BindableProperty NorthColorProperty = BindableProperty.Create(
        nameof(NorthColor),
        typeof(Color),
        typeof(WeatherCompassDial),
        Color.FromArgb("#FF8A80"),
        propertyChanged: OnVisualChanged);

    public static readonly BindableProperty ArrowColorProperty = BindableProperty.Create(
        nameof(ArrowColor),
        typeof(Color),
        typeof(WeatherCompassDial),
        Colors.White,
        propertyChanged: OnVisualChanged);

    public double Direction
    {
        get => (double)GetValue(DirectionProperty);
        set => SetValue(DirectionProperty, value);
    }

    public Color TickColor
    {
        get => (Color)GetValue(TickColorProperty);
        set => SetValue(TickColorProperty, value);
    }

    public Color LabelColor
    {
        get => (Color)GetValue(LabelColorProperty);
        set => SetValue(LabelColorProperty, value);
    }

    public double LabelFontSize
    {
        get => (double)GetValue(LabelFontSizeProperty);
        set => SetValue(LabelFontSizeProperty, value);
    }

    public Color NorthColor
    {
        get => (Color)GetValue(NorthColorProperty);
        set => SetValue(NorthColorProperty, value);
    }

    public Color ArrowColor
    {
        get => (Color)GetValue(ArrowColorProperty);
        set => SetValue(ArrowColorProperty, value);
    }

    //--------------------------------------------------------------------------------
    // Render
    //--------------------------------------------------------------------------------

    private static void OnVisualChanged(BindableObject bindable, object oldValue, object newValue) =>
        ((WeatherCompassDial)bindable).InvalidateSurface();

    protected override void OnPaintSurface(SKPaintSurfaceEventArgs e)
    {
        var canvas = e.Surface.Canvas;
        var info = e.Info;
        canvas.Clear();

        if ((info.Width > 0) && (info.Height > 0) && (Width > 0))
        {
            var density = (float)(info.Width / Width);
            var center = new SKPoint(info.Width / 2f, info.Height / 2f);
            var radius = (MathF.Min(info.Width, info.Height) / 2f) - (DialMargin * density);

            DrawTicks(canvas, center, radius, density);
            DrawLabels(canvas, center, radius - (LabelInset * density), density);
            DrawArrow(canvas, center, radius - (ArrowInset * density), density);
        }
    }

    private void DrawTicks(SKCanvas canvas, SKPoint center, float radius, float density)
    {
        var color = TickColor.ToSKColor();
        using var paint = new SKPaint();
        paint.IsAntialias = true;
        paint.Style = SKPaintStyle.Stroke;
        paint.StrokeCap = SKStrokeCap.Round;
        for (var degree = 0; degree < 360; degree += TickStep)
        {
            var major = (degree % MajorTickStep) == 0;
            paint.StrokeWidth = (major ? MajorTickWidth : MinorTickWidth) * density;
            paint.Color = major ? color : color.WithAlpha((byte)(color.Alpha / 2));
            canvas.DrawLine(PointAt(center, radius, degree), PointAt(center, radius - ((major ? MajorTickLength : MinorTickLength) * density), degree), paint);
        }
    }

    private void DrawLabels(SKCanvas canvas, SKPoint center, float radius, float density)
    {
        using var font = new SKFont(BoldTypeface, (float)LabelFontSize * density);
        using var paint = new SKPaint();
        paint.IsAntialias = true;
        for (var i = 0; i < Labels.Length; i++)
        {
            var point = PointAt(center, radius, i * 90);
            paint.Color = (i == 0 ? NorthColor : LabelColor).ToSKColor();
            var baseline = point.Y - ((font.Metrics.Ascent + font.Metrics.Descent) / 2);
            canvas.DrawText(Labels[i], point.X, baseline, SKTextAlign.Center, font, paint);
        }
    }

    private void DrawArrow(SKCanvas canvas, SKPoint center, float radius, float density)
    {
        var from = Direction;
        var to = Direction + 180;
        var gap = radius * ArrowGapRate;

        using var paint = new SKPaint();
        paint.IsAntialias = true;
        paint.Style = SKPaintStyle.Stroke;
        paint.StrokeWidth = ArrowStrokeWidth * density;
        paint.StrokeCap = SKStrokeCap.Round;
        paint.Color = ArrowColor.ToSKColor();
        var tail = PointAt(center, radius - (TailInset * density), from);
        canvas.DrawLine(tail, PointAt(center, gap, from), paint);
        var head = PointAt(center, radius, to);
        canvas.DrawLine(PointAt(center, gap, to), PointAt(center, radius - (ShaftInset * density), to), paint);

        paint.Style = SKPaintStyle.Fill;
        canvas.DrawCircle(tail, TailRadius * density, paint);

        var back = PointAt(center, radius - (HeadLength * density), to);
        var normal = (float)(to * Math.PI / 180);
        var offset = new SKPoint(MathF.Cos(normal) * HeadHalfWidth * density, MathF.Sin(normal) * HeadHalfWidth * density);
        using var builder = new SKPathBuilder();
        builder.MoveTo(head);
        builder.LineTo(back.X + offset.X, back.Y + offset.Y);
        builder.LineTo(back.X - offset.X, back.Y - offset.Y);
        builder.Close();
        using var path = builder.Detach();
        canvas.DrawPath(path, paint);
    }

    private static SKPoint PointAt(SKPoint center, float radius, double degree)
    {
        var radian = (float)((degree - 90) * Math.PI / 180);
        return new SKPoint(center.X + (MathF.Cos(radian) * radius), center.Y + (MathF.Sin(radian) * radius));
    }
}
