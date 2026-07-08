namespace Template.MobileApp.Modules.View;

using Template.MobileApp.Graphics.Drawing;

#pragma warning disable CA5394
public sealed partial class ViewGraphicsViewModel : AppViewModelBase
{
    private readonly Random random = new();

    public ShapeDrawing Drawing { get; } = new();

    [ObservableProperty]
    public partial int ShapeCount { get; set; }

    public IObserveCommand AddLineCommand { get; }
    public IObserveCommand AddCircleCommand { get; }
    public IObserveCommand AddRectCommand { get; }
    public IObserveCommand ClearCommand { get; }

    public ViewGraphicsViewModel()
    {
        Drawing.Size = new SizeF(100, 100);
        Drawing.Shapes.Add(new Line { Color = Colors.Blue, Point1 = new PointF(10, 10), Point2 = new PointF(90, 90) });
        Drawing.Shapes.Add(new Line { Color = Colors.Blue, Point1 = new PointF(10, 90), Point2 = new PointF(90, 10) });
        Drawing.Shapes.Add(new Rectangle { Color = Colors.Red, Rect = new RectF(40, 40, 20, 20) });
        Drawing.Invalidate();
        ShapeCount = Drawing.Shapes.Count;

        AddLineCommand = MakeDelegateCommand(() => AddShape(new Line
        {
            Color = RandomColor(),
            Width = 2,
            Point1 = RandomPoint(),
            Point2 = RandomPoint()
        }));
        AddCircleCommand = MakeDelegateCommand(() => AddShape(new Circle
        {
            Color = RandomColor(),
            Center = RandomPoint(),
            Radius = (float)((random.NextDouble() * 15) + 5)
        }));
        AddRectCommand = MakeDelegateCommand(() => AddShape(new Rectangle
        {
            Color = RandomColor(),
            Rect = new RectF(RandomPoint(), new SizeF((float)((random.NextDouble() * 20) + 10), (float)((random.NextDouble() * 20) + 10)))
        }));
        ClearCommand = MakeDelegateCommand(() =>
        {
            Drawing.Shapes.Clear();
            Drawing.Invalidate();
            ShapeCount = 0;
        });
    }

    private void AddShape(IShape shape)
    {
        Drawing.Shapes.Add(shape);
        Drawing.Invalidate();
        ShapeCount = Drawing.Shapes.Count;
    }

    private PointF RandomPoint() => new((float)(random.NextDouble() * 100), (float)(random.NextDouble() * 100));

    private Color RandomColor() => Color.FromHsla(random.NextDouble(), 0.7d, 0.5d);

    protected override Task OnNotifyBackAsync() => Navigator.ForwardAsync(ViewId.ViewMenu);

    protected override Task OnNotifyFunction1() => OnNotifyBackAsync();
}
