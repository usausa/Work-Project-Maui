using System.Diagnostics;

namespace WorkSocial;

using SkiaSharp;
using SkiaSharp.Views.Maui;
using SkiaSharp.Views.Maui.Controls;

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

        var rect = new SKRoundRect(e.Info.Rect, 4);
        canvas.ClipRoundRect(rect, SKClipOperation.Intersect, true);

        canvas.Clear(SKColors.Black.WithAlpha(128));

        // TODO
    }
}

//--------------------------------------------------------------------------------
// Player
//--------------------------------------------------------------------------------
public sealed class SocialPlayer : SKCanvasView
{
    public SocialPlayer()
    {
        BackgroundColor = Colors.Transparent;
        PaintSurface += OnPaintSurface;
    }

    private void OnPaintSurface(object? sender, SKPaintSurfaceEventArgs e)
    {
        var canvas = e.Surface.Canvas;
        var info = e.Info;

        Debug.WriteLine($"* Player ({info.Rect.Left},{info.Rect.Top},{info.Rect.Width},{info.Rect.Height}) ({info.Rect.Width / DeviceDisplay.MainDisplayInfo.Density},{info.Rect.Height / DeviceDisplay.MainDisplayInfo.Density})");

        canvas.Clear(SKColors.Black.WithAlpha(128));

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
    public SocialNotification()
    {
        BackgroundColor = Colors.Transparent;
        PaintSurface += OnPaintSurface;
    }

    private void OnPaintSurface(object? sender, SKPaintSurfaceEventArgs e)
    {
        var canvas = e.Surface.Canvas;
        var info = e.Info;

        Debug.WriteLine($"* Notification ({info.Rect.Left},{info.Rect.Top},{info.Rect.Width},{info.Rect.Height}) ({info.Rect.Width / DeviceDisplay.MainDisplayInfo.Density},{info.Rect.Height / DeviceDisplay.MainDisplayInfo.Density})");

        var rect = new SKRoundRect(e.Info.Rect, 4);
        canvas.ClipRoundRect(rect, SKClipOperation.Intersect, true);

        canvas.Clear(SKColors.Black.WithAlpha(128));

        // TODO
    }
}

//--------------------------------------------------------------------------------
// Status
//--------------------------------------------------------------------------------
public sealed class SocialStatus : SKCanvasView
{
    public SocialStatus()
    {
        BackgroundColor = Colors.Transparent;
        PaintSurface += OnPaintSurface;
    }

    private void OnPaintSurface(object? sender, SKPaintSurfaceEventArgs e)
    {
        var canvas = e.Surface.Canvas;
        var info = e.Info;

        Debug.WriteLine($"* Status ({info.Rect.Left},{info.Rect.Top},{info.Rect.Width},{info.Rect.Height}) ({info.Rect.Width / DeviceDisplay.MainDisplayInfo.Density},{info.Rect.Height / DeviceDisplay.MainDisplayInfo.Density})");

        var rect = new SKRoundRect(e.Info.Rect, 4);
        canvas.ClipRoundRect(rect, SKClipOperation.Intersect, true);

        canvas.Clear(SKColors.Black.WithAlpha(128));

        // TODO
    }
}

//--------------------------------------------------------------------------------
// Information
//--------------------------------------------------------------------------------
public sealed class SocialInformation : SKCanvasView
{
    public SocialInformation()
    {
        BackgroundColor = Colors.Transparent;
        PaintSurface += OnPaintSurface;
    }

    private void OnPaintSurface(object? sender, SKPaintSurfaceEventArgs e)
    {
        var canvas = e.Surface.Canvas;
        var info = e.Info;

        Debug.WriteLine($"* Information ({info.Rect.Left},{info.Rect.Top},{info.Rect.Width},{info.Rect.Height}) ({info.Rect.Width / DeviceDisplay.MainDisplayInfo.Density},{info.Rect.Height / DeviceDisplay.MainDisplayInfo.Density})");

        var rect = new SKRoundRect(e.Info.Rect, 4);
        canvas.ClipRoundRect(rect, SKClipOperation.Intersect, true);

        canvas.Clear(SKColors.Black.WithAlpha(128));

        // TODO
    }
}

//--------------------------------------------------------------------------------
// Menu
//--------------------------------------------------------------------------------
public sealed class SocialMenu : SKCanvasView
{
    public SocialMenu()
    {
        BackgroundColor = Colors.Transparent;
        PaintSurface += OnPaintSurface;
    }

    private void OnPaintSurface(object? sender, SKPaintSurfaceEventArgs e)
    {
        var canvas = e.Surface.Canvas;
        var info = e.Info;

        Debug.WriteLine($"* Menu ({info.Rect.Left},{info.Rect.Top},{info.Rect.Width},{info.Rect.Height}) ({info.Rect.Width / DeviceDisplay.MainDisplayInfo.Density},{info.Rect.Height / DeviceDisplay.MainDisplayInfo.Density})");

        canvas.Clear(SKColors.Black.WithAlpha(128));

        //canvas.Clear(SKColors.Black.WithAlpha(128));

        //var paint = new SKPaint { Color = SKColors.Black.WithAlpha(128), Style = SKPaintStyle.Fill };
        //canvas.DrawRect(info.Rect, paint);
        // TODO
    }
}
