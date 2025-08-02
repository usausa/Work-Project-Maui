using System.Diagnostics;

namespace WorkSocial;

using Android.Text;

using Microsoft.Maui;

using SkiaSharp;
using SkiaSharp.Views.Maui;
using SkiaSharp.Views.Maui.Controls;

using static Android.Print.PrintAttributes;
using static System.Net.Mime.MediaTypeNames;

// ReSharper disable InconsistentNaming
public static class FontFaces
{
    public static SKTypeface NotoSerifJP { get; private set; } = default!;

    public static SKTypeface Oxanium { get; private set; } = default!;

    public static SKTypeface Gkktt { get; private set; } = default!;

    public static void Initialize()
    {
        NotoSerifJP = LoadFont("NotoSerifJP-Medium.ttf");
        Oxanium = LoadFont("Oxanium-Regular.ttf");
        Gkktt = LoadFont("851Gkktt_005.ttf");
    }

    private static SKTypeface LoadFont(string fontName)
    {
        using var stream = FileSystem.OpenAppPackageFileAsync(fontName).GetAwaiter().GetResult();
        return SKFontManager.Default.CreateTypeface(stream);
    }
}
// ReSharper restore InconsistentNaming

//--------------------------------------------------------------------------------
// Icon
//--------------------------------------------------------------------------------
public sealed class SocialIcon : SKCanvasView
{
    public SocialIcon()
    {
        BackgroundColor = Colors.Transparent;
        PaintSurface += OnPaintSurface;
    }

    private void OnPaintSurface(object? sender, SKPaintSurfaceEventArgs e)
    {
        var canvas = e.Surface.Canvas;
        var info = e.Info;

        Debug.WriteLine($"* Icon ({info.Rect.Left},{info.Rect.Top},{info.Rect.Width},{info.Rect.Height}) ({info.Rect.Width / DeviceDisplay.MainDisplayInfo.Density},{info.Rect.Height / DeviceDisplay.MainDisplayInfo.Density})");

        var rect = new SKRoundRect(e.Info.Rect, (int)(4 * DeviceDisplay.MainDisplayInfo.Density));
        canvas.ClipRoundRect(rect, SKClipOperation.Intersect, true);

        canvas.Clear(new SKColor(248, 248, 248));

        // TODO
    }
}

//--------------------------------------------------------------------------------
// Player
//--------------------------------------------------------------------------------
public sealed class SocialPlayer : SKCanvasView
{
    public static readonly BindableProperty ColorProperty = BindableProperty.Create(
        nameof(Color),
        typeof(Color),
        typeof(SocialPlayer),
        propertyChanged: Invalidate);

    public Color Color
    {
        get => (Color)GetValue(ColorProperty);
        set => SetValue(ColorProperty, value);
    }

    public SocialPlayer()
    {
        BackgroundColor = Colors.Transparent;
        PaintSurface += OnPaintSurface;
    }

    private static void Invalidate(BindableObject bindable, object oldValue, object newValue)
    {
        ((SocialPlayer)bindable).InvalidateSurface();
    }

    private void OnPaintSurface(object? sender, SKPaintSurfaceEventArgs e)
    {
        var canvas = e.Surface.Canvas;
        var info = e.Info;

        Debug.WriteLine($"* Player ({info.Rect.Left},{info.Rect.Top},{info.Rect.Width},{info.Rect.Height}) ({info.Rect.Width / DeviceDisplay.MainDisplayInfo.Density},{info.Rect.Height / DeviceDisplay.MainDisplayInfo.Density})");

        var leftBorder = (int)(4 * DeviceDisplay.MainDisplayInfo.Density);
        var bottomBorder = (int)(1 * DeviceDisplay.MainDisplayInfo.Density);


        canvas.Clear(SKColors.Black.WithAlpha(128));

        // Border
        using var borderPaint = new SKPaint();
        borderPaint.Color = Color.ToSKColor();
        borderPaint.Style = SKPaintStyle.Fill;
        var leftRect = new SKRect(0, 0, leftBorder, info.Rect.Height);
        canvas.DrawRect(leftRect, borderPaint);
        var borderRect = new SKRect(0, info.Rect.Height - bottomBorder, info.Rect.Width, info.Rect.Height);
        canvas.DrawRect(borderRect, borderPaint);

        // TODO
    }
}

//--------------------------------------------------------------------------------
// Counter
//--------------------------------------------------------------------------------
public sealed class SocialCounter : SKCanvasView
{
    public SocialCounter()
    {
        BackgroundColor = Colors.Transparent;
        PaintSurface += OnPaintSurface;
    }

    private void OnPaintSurface(object? sender, SKPaintSurfaceEventArgs e)
    {
        var canvas = e.Surface.Canvas;
        var info = e.Info;

        Debug.WriteLine($"* Counter ({info.Rect.Left},{info.Rect.Top},{info.Rect.Width},{info.Rect.Height}) ({info.Rect.Width / DeviceDisplay.MainDisplayInfo.Density},{info.Rect.Height / DeviceDisplay.MainDisplayInfo.Density})");

        canvas.Clear(SKColors.Black.WithAlpha(128));

        // TODO
    }
}


//--------------------------------------------------------------------------------
// Notification
//--------------------------------------------------------------------------------
public sealed class SocialNotification : SKCanvasView
{
    public static readonly BindableProperty ColorProperty = BindableProperty.Create(
        nameof(Color),
        typeof(Color),
        typeof(SocialNotification),
        propertyChanged: Invalidate);

    public Color Color
    {
        get => (Color)GetValue(ColorProperty);
        set => SetValue(ColorProperty, value);
    }

    public static readonly BindableProperty Text1Property = BindableProperty.Create(
        nameof(Text1),
        typeof(string),
        typeof(SocialNotification),
        propertyChanged: Invalidate);

    public string Text1
    {
        get => (string)GetValue(Text1Property);
        set => SetValue(Text1Property, value);
    }

    public static readonly BindableProperty Text2Property = BindableProperty.Create(
        nameof(Text2),
        typeof(string),
        typeof(SocialNotification),
        propertyChanged: Invalidate);

    public string Text2
    {
        get => (string)GetValue(Text2Property);
        set => SetValue(Text2Property, value);
    }

    public static readonly BindableProperty Text3Property = BindableProperty.Create(
        nameof(Text3),
        typeof(string),
        typeof(SocialNotification),
        propertyChanged: Invalidate);

    public string Text3
    {
        get => (string)GetValue(Text3Property);
        set => SetValue(Text3Property, value);
    }

    public static readonly BindableProperty PercentProperty = BindableProperty.Create(
        nameof(Percent),
        typeof(double),
        typeof(SocialNotification),
        propertyChanged: Invalidate);

    public double Percent
    {
        get => (double)GetValue(PercentProperty);
        set => SetValue(PercentProperty, value);
    }

    public SocialNotification()
    {
        BackgroundColor = Colors.Transparent;
        PaintSurface += OnPaintSurface;
    }

    private static void Invalidate(BindableObject bindable, object oldValue, object newValue)
    {
        ((SocialNotification)bindable).InvalidateSurface();
    }

    private void OnPaintSurface(object? sender, SKPaintSurfaceEventArgs e)
    {
        var canvas = e.Surface.Canvas;
        var info = e.Info;

        var space = (int)(2 * DeviceDisplay.MainDisplayInfo.Density);
        var margin = (int)(4 * DeviceDisplay.MainDisplayInfo.Density);
        var leftBorder = (int)(4 * DeviceDisplay.MainDisplayInfo.Density);
        var bottomBorder = (int)(3 * DeviceDisplay.MainDisplayInfo.Density);

        using var font1 = new SKFont(FontFaces.Oxanium, size: (int)(12 * DeviceDisplay.MainDisplayInfo.Density));
        using var font2 = new SKFont(FontFaces.NotoSerifJP, size: (int)(14 * DeviceDisplay.MainDisplayInfo.Density));
        using var font3 = new SKFont(FontFaces.Oxanium, size: (int)(10 * DeviceDisplay.MainDisplayInfo.Density));
        var font1Height = (int)Math.Ceiling(-font1.Metrics.Ascent);
        var font2Height = (int)Math.Ceiling(-font2.Metrics.Ascent);
        var font3Height = (int)Math.Ceiling(font3.Metrics.Descent - font3.Metrics.Ascent);
        var titleHeight = font1Height + (margin * 2);

        Debug.WriteLine($"* Notification ({info.Rect.Left},{info.Rect.Top},{info.Rect.Width},{info.Rect.Height}) ({info.Rect.Width / DeviceDisplay.MainDisplayInfo.Density},{info.Rect.Height / DeviceDisplay.MainDisplayInfo.Density})");

        using var paint = new SKPaint();
        paint.Style = SKPaintStyle.Fill;
        paint.IsAntialias = true;

        // Background
        paint.Color = SKColors.Black.WithAlpha(192);
        canvas.DrawRect(new SKRect(0, 0, info.Rect.Right, titleHeight), paint);
        paint.Color = SKColors.Black.WithAlpha(128);
        canvas.DrawRect(new SKRect(0, titleHeight, info.Rect.Right, info.Rect.Height), paint);

        // Border
        paint.Color = Color.ToSKColor();
        canvas.DrawRect(new SKRect(0, 0, leftBorder, info.Rect.Height), paint);

        paint.Color = new SKColor(224, 224, 224).WithAlpha(128);
        canvas.DrawRect(new SKRect(leftBorder, info.Rect.Height - bottomBorder, (float)((info.Rect.Width - leftBorder) * Percent / 100), info.Rect.Height), paint);

        var x = margin + leftBorder;
        var y = margin;

        paint.Color = new SKColor(238, 238, 238);

        y += font1Height;
        canvas.DrawText(Text1, x, y, font1, paint);

        y += space;

        y += font2Height;
        canvas.DrawText(Text2, x, y, font2, paint);
        y += font3Height;
        canvas.DrawText(Text3, x, y, font3, paint);
    }
}

//--------------------------------------------------------------------------------
// Status
//--------------------------------------------------------------------------------
public sealed class SocialStatus : SKCanvasView
{
    public static readonly BindableProperty ColorProperty = BindableProperty.Create(
        nameof(Color),
        typeof(Color),
        typeof(SocialStatus),
        propertyChanged: Invalidate);

    public Color Color
    {
        get => (Color)GetValue(ColorProperty);
        set => SetValue(ColorProperty, value);
    }

    public SocialStatus()
    {
        BackgroundColor = Colors.Transparent;
        PaintSurface += OnPaintSurface;
    }

    private static void Invalidate(BindableObject bindable, object oldValue, object newValue)
    {
        ((SocialStatus)bindable).InvalidateSurface();
    }

    private void OnPaintSurface(object? sender, SKPaintSurfaceEventArgs e)
    {
        var canvas = e.Surface.Canvas;
        var info = e.Info;

        var margin = (int)(2 * DeviceDisplay.MainDisplayInfo.Density);
        var leftBorder = (int)(4 * DeviceDisplay.MainDisplayInfo.Density);

        using var font1 = new SKFont(FontFaces.NotoSerifJP, size: (int)(16 * DeviceDisplay.MainDisplayInfo.Density));
        using var font2 = new SKFont(FontFaces.NotoSerifJP, size: (int)(14 * DeviceDisplay.MainDisplayInfo.Density));
        using var font3 = new SKFont(FontFaces.Oxanium, size: (int)(10 * DeviceDisplay.MainDisplayInfo.Density));

        var font1Height = (int)Math.Ceiling(-font1.Metrics.Ascent);
        var font2Height = (int)Math.Ceiling(-font2.Metrics.Ascent);
        var font3Height = (int)Math.Ceiling(font3.Metrics.Descent - font3.Metrics.Ascent);
        var titleHeight = font1Height + (margin * 2);

        using var paint = new SKPaint();
        paint.Style = SKPaintStyle.Fill;
        paint.IsAntialias = true;

        // Background
        paint.Color = SKColors.Black.WithAlpha(192);
        canvas.DrawRect(new SKRect(0, 0, info.Rect.Right, titleHeight), paint);
        paint.Color = SKColors.Black.WithAlpha(128);
        canvas.DrawRect(new SKRect(0, titleHeight, info.Rect.Right, info.Rect.Height), paint);

        // Border
        paint.Color = Color.ToSKColor();
        var leftRect = new SKRect(0, 0, leftBorder, info.Rect.Height);
        canvas.DrawRect(leftRect, paint);

        var x = leftBorder + margin;
        var y = 0;

        paint.Color = new SKColor(238, 238, 238);

        y += font1Height;
        canvas.DrawText("甲種聖装 瑠璃", x, y, font1, paint);

        // TODO
    }
}

//--------------------------------------------------------------------------------
// Information
//--------------------------------------------------------------------------------
public sealed class SocialInformation : SKCanvasView
{
    public static readonly BindableProperty ColorProperty = BindableProperty.Create(
        nameof(Color),
        typeof(Color),
        typeof(SocialInformation),
        propertyChanged: Invalidate);

    public Color Color
    {
        get => (Color)GetValue(ColorProperty);
        set => SetValue(ColorProperty, value);
    }

    public SocialInformation()
    {
        BackgroundColor = Colors.Transparent;
        PaintSurface += OnPaintSurface;
    }

    private static void Invalidate(BindableObject bindable, object oldValue, object newValue)
    {
        ((SocialInformation)bindable).InvalidateSurface();
    }

    // ReSharper disable StringLiteralTypo
    private void OnPaintSurface(object? sender, SKPaintSurfaceEventArgs e)
    {
        var canvas = e.Surface.Canvas;
        var info = e.Info;

        var margin = (int)(2 * DeviceDisplay.MainDisplayInfo.Density);
        var leftBorder = (int)(4 * DeviceDisplay.MainDisplayInfo.Density);

        using var font1 = new SKFont(FontFaces.NotoSerifJP, size: (int)(16 * DeviceDisplay.MainDisplayInfo.Density));
        using var font2 = new SKFont(FontFaces.NotoSerifJP, size: (int)(14 * DeviceDisplay.MainDisplayInfo.Density));
        using var font3 = new SKFont(FontFaces.Oxanium, size: (int)(10 * DeviceDisplay.MainDisplayInfo.Density));
        var font1Height = (int)Math.Ceiling(-font1.Metrics.Ascent);
        var font2Height = (int)Math.Ceiling(-font2.Metrics.Ascent);
        var font3Height = (int)Math.Ceiling(font3.Metrics.Descent - font3.Metrics.Ascent);
        var titleHeight = font1Height + (margin * 2);

        Debug.WriteLine($"* Information ({info.Rect.Left},{info.Rect.Top},{info.Rect.Width},{info.Rect.Height}) ({info.Rect.Width / DeviceDisplay.MainDisplayInfo.Density},{info.Rect.Height / DeviceDisplay.MainDisplayInfo.Density})");

        using var paint = new SKPaint();
        paint.Style = SKPaintStyle.Fill;
        paint.IsAntialias = true;

        // Background
        paint.Color = SKColors.Black.WithAlpha(192);
        canvas.DrawRect(new SKRect(0, 0, info.Rect.Right, titleHeight), paint);
        paint.Color = SKColors.Black.WithAlpha(128);
        canvas.DrawRect(new SKRect(0, titleHeight, info.Rect.Right, info.Rect.Height), paint);

        // Border
        paint.Color = Color.ToSKColor();
        canvas.DrawRect(new SKRect(0, 0, leftBorder, info.Rect.Height), paint);

        var x = leftBorder + margin;
        var y = 0;

        paint.Color = new SKColor(238, 238, 238);

        y += font1Height;
        canvas.DrawText("投入戦力 支援部隊", x, y, font1, paint);

        y += margin * 2;

        y += font2Height;
        canvas.DrawText("辺境伯直属戦術機甲大隊", x, y, font2, paint);
        y += font3Height;
        canvas.DrawText("WOLF grp", x, y, font3, paint);
        canvas.DrawText("MF-4000 x36", info.Rect.Right - margin - font3.MeasureText("MF-4000 x36"), y, font3, paint);

        y += margin;

        y += font2Height;
        canvas.DrawText("第二騎士団聖女計画特務中隊", x, y, font2, paint);
        y += font3Height;
        canvas.DrawText("HOUND sqd", x, y, font3, paint);
        canvas.DrawText("TYPE-19E x10 + JXD-20", info.Rect.Right - margin - font3.MeasureText("TYPE-19E x10 + JXD-20"), y, font3, paint);

        y += margin;

        y += font2Height;
        canvas.DrawText("第三騎士団突撃前衛部隊", x, y, font2, paint);
        y += font3Height;
        canvas.DrawText("VIPPERS", x, y, font3, paint);
        canvas.DrawText("TYPE-19 Blood x8", info.Rect.Right - margin - font3.MeasureText("TYPE-19 Blood x8"), y, font3, paint);
    }
    // ReSharper restore StringLiteralTypo
}

//--------------------------------------------------------------------------------
// Menu
//--------------------------------------------------------------------------------
public sealed class SocialMenu : SKCanvasView
{
    public static readonly BindableProperty ColorProperty = BindableProperty.Create(
        nameof(Color),
        typeof(Color),
        typeof(SocialMenu),
        propertyChanged: Invalidate);

    public Color Color
    {
        get => (Color)GetValue(ColorProperty);
        set => SetValue(ColorProperty, value);
    }

    public static readonly BindableProperty Text1Property = BindableProperty.Create(
        nameof(Text1),
        typeof(string),
        typeof(SocialMenu),
        propertyChanged: Invalidate);

    public string Text1
    {
        get => (string)GetValue(Text1Property);
        set => SetValue(Text1Property, value);
    }

    public static readonly BindableProperty Text2Property = BindableProperty.Create(
        nameof(Text2),
        typeof(string),
        typeof(SocialMenu),
        propertyChanged: Invalidate);

    public string Text2
    {
        get => (string)GetValue(Text2Property);
        set => SetValue(Text2Property, value);
    }

    public static readonly BindableProperty FontSize1Property = BindableProperty.Create(
        nameof(FontSize1),
        typeof(double),
        typeof(SocialMenu),
        24d,
        propertyChanged: Invalidate);

    public double FontSize1
    {
        get => (double)GetValue(FontSize1Property);
        set => SetValue(FontSize1Property, value);
    }

    public static readonly BindableProperty FontSize2Property = BindableProperty.Create(
        nameof(FontSize2),
        typeof(double),
        typeof(SocialMenu),
        10d,
        propertyChanged: Invalidate);

    public double FontSize2
    {
        get => (double)GetValue(FontSize2Property);
        set => SetValue(FontSize2Property, value);
    }

    public SocialMenu()
    {
        BackgroundColor = Colors.Transparent;
        PaintSurface += OnPaintSurface;
    }

    private static void Invalidate(BindableObject bindable, object oldValue, object newValue)
    {
        ((SocialMenu)bindable).InvalidateSurface();
    }

    private void OnPaintSurface(object? sender, SKPaintSurfaceEventArgs e)
    {
        var canvas = e.Surface.Canvas;
        var info = e.Info;

        Debug.WriteLine($"* Menu ({info.Rect.Left},{info.Rect.Top},{info.Rect.Width},{info.Rect.Height}) ({info.Rect.Width / DeviceDisplay.MainDisplayInfo.Density},{info.Rect.Height / DeviceDisplay.MainDisplayInfo.Density})");

        var border = (int)(2 * DeviceDisplay.MainDisplayInfo.Density);
        var leftBorder = (int)(8 * DeviceDisplay.MainDisplayInfo.Density);
        var clientRect = new SKRect(leftBorder, border, info.Rect.Width - border, info.Rect.Height - border);

        var text1 = Text1;
        var text2 = Text2;
        using var font1 = new SKFont(FontFaces.NotoSerifJP, size: (int)(FontSize1 * DeviceDisplay.MainDisplayInfo.Density));
        using var font2 = new SKFont(FontFaces.Oxanium, size: (int)(10 * DeviceDisplay.MainDisplayInfo.Density));
        var text2Height = (int)Math.Ceiling(font2.Metrics.Descent - font2.Metrics.Ascent);

        // Background
        canvas.Save();
        canvas.ClipRect(clientRect, SKClipOperation.Intersect, true);
        canvas.Clear(SKColors.Black.WithAlpha(160));
        canvas.Restore();

        // Border
        canvas.Save();
        canvas.ClipRect(clientRect, SKClipOperation.Difference, true);
        canvas.Clear(new SKColor(158, 158, 158).WithAlpha(192));
        canvas.Restore();

        using var paint = new SKPaint();
        paint.Color = Color.ToSKColor();
        paint.Style = SKPaintStyle.Fill;
        var leftRect = new SKRect(0, 0, leftBorder, info.Rect.Height);
        canvas.DrawRect(leftRect, paint);

        // Text1
        using var text1Paint = new SKPaint();
        text1Paint.Color = new SKColor(238, 238, 238);
        text1Paint.IsAntialias = true;

        var text1Width = font1.MeasureText(text1, text1Paint);
        var text1X = (int)((clientRect.Width - text1Width) / 2) + leftBorder;
        var text1H = (int)Math.Ceiling(-font1.Metrics.Ascent);
        var text1Y = ((info.Rect.Height - text2Height - (border * 5)) / 2) + (text1H / 2);
        canvas.DrawText(text1, text1X, text1Y, font1, text1Paint);

        // Text2

        using var text2Paint = new SKPaint();
        text2Paint.Color = new SKColor(238, 238, 238);
        text2Paint.IsAntialias = true;

        var text2Width = font2.MeasureText(text2, text2Paint);
        var text2X = (int)((clientRect.Width - text2Width) / 2) + leftBorder;
        canvas.DrawText(text2, text2X, info.Rect.Height - (border * 3), font2, text2Paint);
    }
}
