namespace GraphicExample;

public sealed class TestDrawable : IDrawable
{
    private readonly TestData data;

    public TestDrawable(TestData data)
    {
        this.data = data;
    }

    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        // Draw border
        //canvas.DrawRectangle(dirtyRect.Inflate(-1, -1));

        canvas.FillColor = Colors.Black;
        foreach (var point in data.Points)
        {
            canvas.FillCircle(new Point(dirtyRect.Width * point.X, dirtyRect.Height * point.Y), 15d);
        }
    }
}
