namespace WorkSizeMap;

using System.Diagnostics;

public partial class MainPage : ContentPage, IDrawable
{
    public MainPage()
    {
        InitializeComponent();

        GraphicsView.Drawable = this;
    }

    private void ImageOnLoaded(object? sender, EventArgs e)
    {
        if (sender is Image image)
        {
            Debug.WriteLine("* Onloaded");
            Debug.WriteLine($"Size: {image.Width}x{image.Height}");
            Debug.WriteLine($"Actual: {image.Bounds.Width}x{image.Bounds.Height}");
            Debug.WriteLine($"DesiredSize: {image.DesiredSize}");
        }
    }

    private void ImageOnSizeChanged(object? sender, EventArgs e)
    {
        if (sender is Image image)
        {
            Debug.WriteLine("* Onloaded");
            Debug.WriteLine($"Size: {image.Width}x{image.Height}");
            Debug.WriteLine($"Actual: {image.Bounds.Width}x{image.Bounds.Height}");
            Debug.WriteLine($"DesiredSize: {image.DesiredSize}");
        }
    }

    private void Button_OnClicked(object? sender, EventArgs e)
    {
    }

    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        var layoutWidth = dirtyRect.Width;
        var layoutHeight = dirtyRect.Height;

        var imageWidth = 1080f;
        var imageHeight = 1080f;

        var scale = Math.Min(layoutWidth / imageWidth, layoutHeight / imageHeight);

        var displayWidth = imageWidth * scale;
        var displayHeight = imageHeight * scale;

        var offsetX = (layoutWidth - displayWidth) / 2;
        var offsetY = (layoutHeight - displayHeight) / 2;

        canvas.StrokeColor = Colors.Red;
        canvas.DrawRectangle(offsetX, offsetY, displayWidth, displayHeight);
    }
}
