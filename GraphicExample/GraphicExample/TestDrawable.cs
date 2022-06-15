namespace GraphicExample;

internal class TestDrawable : IDrawable
{
    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        // Draw border
        canvas.DrawRectangle(dirtyRect.Inflate(-1, -1));

        canvas.FillColor = Colors.Black;
        canvas.FillCircle(new Point(dirtyRect.Width / 2, dirtyRect.Height / 2), 15d);
    }
}
