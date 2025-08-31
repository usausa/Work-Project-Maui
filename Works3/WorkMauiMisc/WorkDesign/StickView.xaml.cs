namespace WorkDesign;

using Microsoft.Maui;
using Microsoft.Maui.Controls;

public partial class StickView : ContentView
{
    public static readonly BindableProperty BaseBackgroundProperty = BindableProperty.Create(
        nameof(BaseBackground),
        typeof(Brush),
        typeof(StickView),
        propertyChanged: UpdateBase);

    public Brush? BaseBackground
    {
        get => (Brush?)GetValue(BaseBackgroundProperty);
        set => SetValue(BaseBackgroundProperty, value);
    }

    public StickView()
	{
		InitializeComponent();

        BaseGraphics.Drawable = new BaseDrawable(this);
        //KnobGraphics.Drawable = new KnobDrawable(this);

        SizeChanged += OnSizeChanged;

        var panGesture = new PanGestureRecognizer();
        panGesture.PanUpdated += OnPanUpdated;
        Knob.GestureRecognizers.Add(panGesture);
        //KnobGraphics.GestureRecognizers.Add(panGesture);
    }

    private static void UpdateBase(BindableObject bindable, object oldValue, object newValue)
    {
        ((StickView)bindable).BaseGraphics.Invalidate();
    }

    private static void UpdateKnob(BindableObject bindable, object oldValue, object newValue)
    {
        //((StickView)bindable).KnobGraphics.Invalidate();
    }

    private void OnSizeChanged(object? sender, EventArgs e)
    {
    }

    private double radius = 80;

    private void OnPanUpdated(object? sender, PanUpdatedEventArgs e)
    {
        switch (e.StatusType)
        {
            case GestureStatus.Started:
            case GestureStatus.Running:
                var (x, y) = ClampToCircle(e.TotalX, e.TotalY);
                Knob.TranslationX = x;
                Knob.TranslationY = y;
                break;

            case GestureStatus.Completed:
            case GestureStatus.Canceled:
                Knob.TranslationX = 0;
                Knob.TranslationY = 0;
                // TODO Update position 0
                break;
        }
    }

    public (double X, double Y) ClampToCircle(double x, double y)
    {
        // 現在の座標と中心(0,0)の距離を計算
        double distance = Math.Sqrt(x * x + y * y);

        // 距離が半径より大きい場合にのみ補正を行う
        if (distance > radius)
        {
            // 正規化されたベクトルに半径を乗算して円周上の座標を求める
            double newX = x * (radius / distance);
            double newY = y * (radius / distance);
            return (newX, newY);
        }
        else
        {
            // 範囲内の場合はそのままの座標を返す
            return (x, y);
        }
    }
    //private bool HitTest(double x, double y)
    //{
    //    return x * x + y * y <= KnobGraphics.Width * KnobGraphics.Width;
    //}

    //--------------------------------------------------------------------------------

    private sealed class KnobDrawable : IDrawable
    {
        private readonly StickView owner;

        public KnobDrawable(StickView owner)
        {
            this.owner = owner;
        }

        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            canvas.SaveState();
            canvas.Antialias = true;

            var cx = dirtyRect.Center.X;
            var cy = dirtyRect.Center.Y;
            var size = MathF.Min(dirtyRect.Width, dirtyRect.Height);
            var radius = size / 2;

            var circleRect = new RectF(cx - radius, cy - radius, size, size);

            var gradient = new LinearGradientPaint
            {
                // 左上から右下へのグラデーション
                StartPoint = new Point(0, 0),
                EndPoint = new Point(1, 1),
                // グラデーションの色の設定
                GradientStops =
                [
                    new PaintGradientStop(0f, Color.FromArgb("#8888ff")),
                    new PaintGradientStop(1f, Color.FromArgb("#6666ff"))
                ]
            };

            canvas.SetFillPaint(gradient, circleRect);
            canvas.FillEllipse(circleRect);

            canvas.RestoreState();
        }
    }

    //--------------------------------------------------------------------------------

    private sealed class BaseDrawable : IDrawable
    {
        public Color TriangleColor { get; set; } = Color.FromArgb("#444444");

        public float MarginRatio { get; set; } = 0.1f;
        public float TriangleHeightRatio { get; set; } = 0.2f;
        public float TriangleHalfWidthRatio { get; set; } = 0.2f;

        private readonly StickView owner;

        public BaseDrawable(StickView owner)
        {
            this.owner = owner;
        }

        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            canvas.SaveState();
            canvas.Antialias = true;

            var cx = dirtyRect.Center.X;
            var cy = dirtyRect.Center.Y;
            var size = MathF.Min(dirtyRect.Width, dirtyRect.Height);
            var radius = size / 2;

            var circleRect = new RectF(cx - radius, cy - radius, size, size);

            var gradient = new LinearGradientPaint
            {
                // 左上から右下へのグラデーション
                StartPoint = new Point(0, 0),
                EndPoint = new Point(1, 1),
                // グラデーションの色の設定
                GradientStops =
                [
                    new PaintGradientStop(0f, Color.FromArgb("#666666")),
                    new PaintGradientStop(1f, Color.FromArgb("#888888"))
                ]
            };

            canvas.SetFillPaint(gradient, circleRect);
            canvas.FillEllipse(circleRect);

            canvas.RestoreState();

            // 円と係数から三角形の座標を計算（例で説明されたルール）

            var dirMargin = radius * MarginRatio;
            var dirHeight = radius * TriangleHeightRatio;
            var dirWidth = radius * TriangleHalfWidthRatio;

            canvas.FillColor = TriangleColor;

            // Top
            var apexT = new PointF(cx, circleRect.Top + dirMargin);
            FillTriangle(canvas, apexT, new PointF(cx - dirWidth, apexT.Y + dirHeight), new PointF(cx + dirWidth, apexT.Y + dirHeight));
            // Bottom
            var apexB = new PointF(cx, circleRect.Bottom - dirMargin);
            FillTriangle(canvas, apexB, new PointF(cx - dirWidth, apexB.Y - dirHeight), new PointF(cx + dirWidth, apexB.Y - dirHeight));
            // Left
            var apexL = new PointF(circleRect.Left + dirMargin, cy);
            FillTriangle(canvas, apexL, new PointF(apexL.X + dirHeight, cy - dirWidth), new PointF(apexL.X + dirHeight, cy + dirWidth));
            // Right
            var apexR = new PointF(circleRect.Right - dirMargin, cy);
            FillTriangle(canvas, apexR, new PointF(apexR.X - dirHeight, cy - dirWidth), new PointF(apexR.X - dirHeight, cy + dirWidth));

            return;

            // TODO ext
            static void FillTriangle(ICanvas canvas, PointF a, PointF b, PointF c)
            {
                var path = new PathF();
                path.MoveTo(a);
                path.LineTo(b);
                path.LineTo(c);
                path.Close();
                canvas.FillPath(path);
            }
        }
    }
}