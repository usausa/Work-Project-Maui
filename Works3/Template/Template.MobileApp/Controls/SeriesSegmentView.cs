namespace Template.MobileApp.Controls;

using SkiaSharp;
using SkiaSharp.Views.Maui;
using SkiaSharp.Views.Maui.Controls;

public sealed class SeriesSegmentView : SKCanvasView
{
    private const string RippleAnimationName = "SeriesRipple";

    private const string RevealAnimationName = "SeriesReveal";

    private const int RevealLength = 80;

    private const uint AnimationRate = 16;

    private const uint RippleRate = 32;

    private const uint RippleLength = 1800;

    private const float TextSpacing = 12f;

    private const float TextOffset = 10f;

    private const float PointMargin = 8f;

    private const float LineWidth = 2f;

    private const float PointRadius = 3f;

    private const float HighlightedPointRadius = 4.5f;

    private const float HaloRadius = 8f;

    private const float RippleWidth = 1.5f;

    private const float RippleSpread = 10f;

    private const float RippleAlpha = 200f;

    private const byte HaloAlpha = 90;

    private static readonly SKTypeface BoldTypeface = SKTypeface.FromFamilyName("sans-serif", SKFontStyle.Bold);

    //--------------------------------------------------------------------------------
    // Member
    //--------------------------------------------------------------------------------

    private float ripple;

    private float reveal = 1f;

    //--------------------------------------------------------------------------------
    // Property
    //--------------------------------------------------------------------------------

    public static readonly BindableProperty ValueProperty = BindableProperty.Create(
        nameof(Value),
        typeof(double),
        typeof(SeriesSegmentView),
        0d,
        propertyChanged: OnVisualChanged);

    public static readonly BindableProperty PreviousProperty = BindableProperty.Create(
        nameof(Previous),
        typeof(double?),
        typeof(SeriesSegmentView),
        propertyChanged: OnVisualChanged);

    public static readonly BindableProperty NextProperty = BindableProperty.Create(
        nameof(Next),
        typeof(double?),
        typeof(SeriesSegmentView),
        propertyChanged: OnVisualChanged);

    public static readonly BindableProperty MinimumProperty = BindableProperty.Create(
        nameof(Minimum),
        typeof(double),
        typeof(SeriesSegmentView),
        0d,
        propertyChanged: OnVisualChanged);

    public static readonly BindableProperty MaximumProperty = BindableProperty.Create(
        nameof(Maximum),
        typeof(double),
        typeof(SeriesSegmentView),
        1d,
        propertyChanged: OnVisualChanged);

    public static readonly BindableProperty TextProperty = BindableProperty.Create(
        nameof(Text),
        typeof(string),
        typeof(SeriesSegmentView),
        propertyChanged: OnVisualChanged);

    public static readonly BindableProperty FontSizeProperty = BindableProperty.Create(
        nameof(FontSize),
        typeof(double),
        typeof(SeriesSegmentView),
        14d,
        propertyChanged: OnVisualChanged);

    public static readonly BindableProperty TextColorProperty = BindableProperty.Create(
        nameof(TextColor),
        typeof(Color),
        typeof(SeriesSegmentView),
        Colors.White,
        propertyChanged: OnVisualChanged);

    public static readonly BindableProperty LineColorProperty = BindableProperty.Create(
        nameof(LineColor),
        typeof(Color),
        typeof(SeriesSegmentView),
        Colors.White,
        propertyChanged: OnVisualChanged);

    public static readonly BindableProperty FillColorProperty = BindableProperty.Create(
        nameof(FillColor),
        typeof(Color),
        typeof(SeriesSegmentView),
        Color.FromRgba(255, 255, 255, 64),
        propertyChanged: OnVisualChanged);

    public static readonly BindableProperty IsHighlightedProperty = BindableProperty.Create(
        nameof(IsHighlighted),
        typeof(bool),
        typeof(SeriesSegmentView),
        false,
        propertyChanged: OnHighlightedChanged);

    public static readonly BindableProperty RevealOrderProperty = BindableProperty.Create(
        nameof(RevealOrder),
        typeof(int),
        typeof(SeriesSegmentView),
        -1);

    private static readonly BindableProperty RevealOriginProperty = BindableProperty.CreateAttached(
        "RevealOrigin",
        typeof(long),
        typeof(SeriesSegmentView),
        0L);

    public double Value
    {
        get => (double)GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    public double? Previous
    {
        get => (double?)GetValue(PreviousProperty);
        set => SetValue(PreviousProperty, value);
    }

    public double? Next
    {
        get => (double?)GetValue(NextProperty);
        set => SetValue(NextProperty, value);
    }

    public double Minimum
    {
        get => (double)GetValue(MinimumProperty);
        set => SetValue(MinimumProperty, value);
    }

    public double Maximum
    {
        get => (double)GetValue(MaximumProperty);
        set => SetValue(MaximumProperty, value);
    }

    public string? Text
    {
        get => (string?)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public double FontSize
    {
        get => (double)GetValue(FontSizeProperty);
        set => SetValue(FontSizeProperty, value);
    }

    public Color TextColor
    {
        get => (Color)GetValue(TextColorProperty);
        set => SetValue(TextColorProperty, value);
    }

    public Color LineColor
    {
        get => (Color)GetValue(LineColorProperty);
        set => SetValue(LineColorProperty, value);
    }

    public Color FillColor
    {
        get => (Color)GetValue(FillColorProperty);
        set => SetValue(FillColorProperty, value);
    }

    public bool IsHighlighted
    {
        get => (bool)GetValue(IsHighlightedProperty);
        set => SetValue(IsHighlightedProperty, value);
    }

    public int RevealOrder
    {
        get => (int)GetValue(RevealOrderProperty);
        set => SetValue(RevealOrderProperty, value);
    }

    //--------------------------------------------------------------------------------
    // Constructor
    //--------------------------------------------------------------------------------

    public SeriesSegmentView()
    {
        Loaded += (_, _) =>
        {
            UpdateRipple();
            StartReveal();
        };
        Unloaded += (_, _) =>
        {
            this.AbortAnimation(RippleAnimationName);
            this.AbortAnimation(RevealAnimationName);
        };
    }

    //--------------------------------------------------------------------------------
    // Render
    //--------------------------------------------------------------------------------

    private static void OnVisualChanged(BindableObject bindable, object oldValue, object newValue) =>
        ((SeriesSegmentView)bindable).InvalidateSurface();

    protected override void OnPaintSurface(SKPaintSurfaceEventArgs e)
    {
        var canvas = e.Surface.Canvas;
        var info = e.Info;
        canvas.Clear();

        if ((info.Width > 0) && (info.Height > 0) && (Width > 0))
        {
            var density = (float)(info.Width / Width);
            using var font = new SKFont(BoldTypeface, (float)FontSize * density);

            var top = (font.Metrics.Descent - font.Metrics.Ascent) + (TextSpacing * density);
            var bottom = info.Height - (PointMargin * density);
            var center = new SKPoint(info.Width / 2f, ToY(Value, top, bottom));
            var left = new SKPoint(0, Previous is { } previous ? ToY((previous + Value) / 2, top, bottom) : center.Y);
            var right = new SKPoint(info.Width, Next is { } next ? ToY((Value + next) / 2, top, bottom) : center.Y);
            var start = Previous is null ? center : left;
            var end = Next is null ? center : right;

            canvas.Save();
            canvas.ClipRect(new SKRect(0, 0, info.Width * reveal, info.Height));
            DrawFill(canvas, start, center, end, top, info.Height);
            DrawLine(canvas, start, center, end, density);
            canvas.Restore();

            if (reveal >= 0.5f)
            {
                DrawPoint(canvas, center, density);
                DrawText(canvas, center, font, density);
            }
        }
    }

    private float ToY(double value, float top, float bottom)
    {
        var range = Maximum - Minimum;
        var ratio = range > 0 ? Math.Clamp((value - Minimum) / range, 0, 1) : 0.5;
        return bottom - (float)(ratio * (bottom - top));
    }

    private void DrawFill(SKCanvas canvas, SKPoint start, SKPoint center, SKPoint end, float top, float height)
    {
        using var builder = new SKPathBuilder();
        builder.MoveTo(start);
        builder.LineTo(center);
        builder.LineTo(end);
        builder.LineTo(end.X, height);
        builder.LineTo(start.X, height);
        builder.Close();
        using var path = builder.Detach();

        var color = FillColor.ToSKColor();
        using var shader = SKShader.CreateLinearGradient(
            new SKPoint(0, top),
            new SKPoint(0, height),
            [color, color.WithAlpha(0)],
            SKShaderTileMode.Clamp);
        using var paint = new SKPaint();
        paint.IsAntialias = true;
        paint.Shader = shader;
        canvas.DrawPath(path, paint);
    }

    private void DrawLine(SKCanvas canvas, SKPoint start, SKPoint center, SKPoint end, float density)
    {
        using var paint = new SKPaint();
        paint.IsAntialias = true;
        paint.Style = SKPaintStyle.Stroke;
        paint.StrokeWidth = LineWidth * density;
        paint.Color = LineColor.ToSKColor();
        canvas.DrawLine(start, center, paint);
        canvas.DrawLine(center, end, paint);
    }

    private void DrawPoint(SKCanvas canvas, SKPoint center, float density)
    {
        var color = LineColor.ToSKColor();
        using var paint = new SKPaint();
        paint.IsAntialias = true;
        if (IsHighlighted)
        {
            paint.Color = color.WithAlpha(HaloAlpha);
            canvas.DrawCircle(center, HaloRadius * density, paint);

            if (ripple > 0f)
            {
                paint.Style = SKPaintStyle.Stroke;
                paint.StrokeWidth = RippleWidth * density;
                paint.Color = color.WithAlpha((byte)(RippleAlpha * (1f - ripple)));
                canvas.DrawCircle(center, (HaloRadius + (RippleSpread * ripple)) * density, paint);
                paint.Style = SKPaintStyle.Fill;
            }
        }

        paint.Color = color;
        canvas.DrawCircle(center, (IsHighlighted ? HighlightedPointRadius : PointRadius) * density, paint);
    }

    private void DrawText(SKCanvas canvas, SKPoint center, SKFont font, float density)
    {
        if (!String.IsNullOrEmpty(Text))
        {
            using var paint = new SKPaint();
            paint.IsAntialias = true;
            paint.Color = TextColor.ToSKColor();
            canvas.DrawText(Text, center.X, center.Y - (TextOffset * density) - font.Metrics.Descent, SKTextAlign.Center, font, paint);
        }
    }

    //--------------------------------------------------------------------------------
    // Animation
    //--------------------------------------------------------------------------------

    private static void OnHighlightedChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (SeriesSegmentView)bindable;
        control.UpdateRipple();
        control.InvalidateSurface();
    }

    private void UpdateRipple()
    {
        this.AbortAnimation(RippleAnimationName);
        ripple = 0f;
        if (IsHighlighted && IsLoaded)
        {
            this.Animate(
                RippleAnimationName,
                v =>
                {
                    ripple = (float)v;
                    InvalidateSurface();
                },
                0d,
                1d,
                RippleRate,
                RippleLength,
                Easing.CubicOut,
                repeat: () => IsHighlighted);
        }
    }

    private void StartReveal()
    {
        this.AbortAnimation(RevealAnimationName);
        reveal = 1f;

        var owner = FindItemsView();
        if ((RevealOrder >= 0) && (owner is not null))
        {
            var now = Environment.TickCount64;
            var origin = (long)owner.GetValue(RevealOriginProperty);
            if (origin == 0)
            {
                origin = now;
                owner.SetValue(RevealOriginProperty, origin);
            }

            var wait = origin + ((long)RevealOrder * RevealLength) - now;
            if (wait + RevealLength > 0)
            {
                var length = Math.Max(wait, 0) + RevealLength;
                reveal = (float)Math.Clamp(-wait / (double)RevealLength, 0, 1);
                this.Animate(
                    RevealAnimationName,
                    v =>
                    {
                        reveal = (float)Math.Clamp(((v * length) - wait) / RevealLength, 0, 1);
                        InvalidateSurface();
                    },
                    0d,
                    1d,
                    AnimationRate,
                    (uint)length,
                    Easing.Linear,
                    (_, _) =>
                    {
                        reveal = 1f;
                        InvalidateSurface();
                    });
            }
        }
    }

    private ItemsView? FindItemsView()
    {
        for (var element = Parent; element is not null; element = element.Parent)
        {
            if (element is ItemsView view)
            {
                return view;
            }
        }

        return null;
    }
}
