namespace Template.MobileApp.Graphics;

using Template.MobileApp.Usecase;

public sealed class DetectGraphics : GraphicsObject
{
    private IReadOnlyList<DetectResult> results = [];

    public void Update(IReadOnlyList<DetectResult> values)
    {
        results = values;
        Invalidate();
    }

    public override void Draw(ICanvas canvas, RectF dirtyRect)
    {
        if (results.Count > 0)
        {
            canvas.StrokeSize = 3;
            canvas.StrokeColor = Colors.Red;

            foreach (var result in results)
            {
                canvas.DrawRectangle(
                    dirtyRect.Width * result.Left,
                    dirtyRect.Height * result.Top,
                    dirtyRect.Width * (result.Right - result.Left),
                    dirtyRect.Height * (result.Bottom - result.Top));
            }
        }
    }
}
