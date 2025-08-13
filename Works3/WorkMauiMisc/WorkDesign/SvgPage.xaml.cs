using SkiaSharp;
using SkiaSharp.Views.Maui;
using SkiaSharp.Views.Maui.Controls;

using Svg.Skia;

using Smart.Maui.ViewModels;
using Smart.Mvvm;

namespace WorkDesign;

public partial class SvgPage : ContentPage
{
	public SvgPage()
	{
		InitializeComponent();
	}
}

public sealed partial class SvgPageViewModel : ExtendViewModelBase
{
    [ObservableProperty]
    public partial SKSvg? Svg { get; set; }

    public SvgPageViewModel()
    {
        var svg = new SKSvg();
        using var stream = FileSystem.OpenAppPackageFileAsync("dotnet_bot.svg").Result;
        svg.Load(stream);

        Svg = svg;
    }
}

public sealed class SvgView : SKCanvasView
{
    public static readonly BindableProperty SvgProperty = BindableProperty.Create(
        nameof(Svg),
        typeof(SKSvg),
        typeof(SvgView),
        propertyChanged: Invalidate);

    public SKSvg? Svg
    {
        get => (SKSvg?)GetValue(SvgProperty);
        set => SetValue(SvgProperty, value);
    }

    public SvgView()
    {
        PaintSurface += OnPaintSurface;
    }

    private static void Invalidate(BindableObject bindable, object oldValue, object newValue)
    {
        ((SvgView)bindable).InvalidateSurface();
    }


    private void OnPaintSurface(object? sender, SKPaintSurfaceEventArgs e)
    {
        var surface = e.Surface;
        var canvas = surface.Canvas;

        if (BackgroundColor is not null)
        {
            canvas.Clear(BackgroundColor.ToSKColor());
        }

        var svg = Svg;
        if (svg?.Picture is null)
        {
            return;
        }

        var x = (e.Info.Width - svg.Picture.CullRect.Width) / 2;
        var y = (e.Info.Height - svg.Picture.CullRect.Height) / 2;
        var canvasMin = Math.Min(e.Info.Width, e.Info.Height);
        var svgMax = Math.Max(svg.Picture.CullRect.Width, svg.Picture.CullRect.Height);
        var scale = canvasMin / svgMax;
        var matrix = SKMatrix.CreateScale(scale, scale);

        canvas.Save();
        canvas.Translate(x, y);
        canvas.DrawPicture(svg.Picture, matrix);
        canvas.Restore();
    }
}
