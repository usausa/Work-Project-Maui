namespace WorkGraphicMvvm;

using Smart.Mvvm;

public partial class SampleGraphics : ObservableObject, IGraphics
{
    [ObservableProperty]
    public partial Color Color { get; set; }

    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        canvas.Antialias = true;
        canvas.StrokeColor = Color;
        canvas.StrokeSize = 5;
        canvas.DrawEllipse(dirtyRect);
    }
}
