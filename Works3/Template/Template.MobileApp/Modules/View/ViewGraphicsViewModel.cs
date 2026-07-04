namespace Template.MobileApp.Modules.View;

using Template.MobileApp.Graphics.Drawing;

public sealed class ViewGraphicsViewModel : AppViewModelBase
{
    public ShapeDrawing Drawing { get; } = new();

    public ViewGraphicsViewModel()
    {
        Drawing.Size = new SizeF(100, 100);
        Drawing.Shapes.Add(new Line { Color = Colors.Blue, Point1 = new PointF(10, 10), Point2 = new PointF(90, 90) });
        Drawing.Shapes.Add(new Line { Color = Colors.Blue, Point1 = new PointF(10, 90), Point2 = new PointF(90, 10) });
        Drawing.Shapes.Add(new Rectangle { Color = Colors.Red, Rect = new RectF(40, 40, 20, 20) });
        Drawing.Invalidate();
    }

    protected override Task OnNotifyBackAsync() => Navigator.ForwardAsync(ViewId.ViewMenu);

    protected override Task OnNotifyFunction1() => OnNotifyBackAsync();
}
