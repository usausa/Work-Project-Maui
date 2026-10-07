namespace Template.MobileApp.Controls;

// Easing 曲線を背景に描画する(横=時間 0→1、縦=出力 0→1。Spring/Bounce のオーバーシュートに備えて上下に余白を取る)
// Progress の時間の位置に、曲線の色の丸を描く
public sealed class EasingCurveView : GraphicsView, IDrawable
{
    private const float BallRadius = 8f;

    public static readonly BindableProperty EasingProperty = BindableProperty.Create(
        nameof(Easing),
        typeof(Easing),
        typeof(EasingCurveView),
        Easing.Linear,
        propertyChanged: Invalidate);

    public Easing Easing
    {
        get => (Easing)GetValue(EasingProperty);
        set => SetValue(EasingProperty, value);
    }

    // 既定値は BlueLighten3 相当
    public static readonly BindableProperty LineColorProperty = BindableProperty.Create(
        nameof(LineColor),
        typeof(Color),
        typeof(EasingCurveView),
        Color.FromArgb("#90CAF9"),
        propertyChanged: Invalidate);

    public Color LineColor
    {
        get => (Color)GetValue(LineColorProperty);
        set => SetValue(LineColorProperty, value);
    }

    public static readonly BindableProperty ProgressProperty = BindableProperty.Create(
        nameof(Progress),
        typeof(double),
        typeof(EasingCurveView),
        0d,
        propertyChanged: Invalidate);

    public double Progress
    {
        get => (double)GetValue(ProgressProperty);
        set => SetValue(ProgressProperty, value);
    }

    private static readonly Color GuideColor = Color.FromArgb("#E0E0E0");

    public EasingCurveView()
    {
        Drawable = this;
        InputTransparent = true;
    }

    private static void Invalidate(BindableObject bindable, object oldValue, object newValue)
    {
        ((EasingCurveView)bindable).Invalidate();
    }

    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        var width = dirtyRect.Width;
        var height = dirtyRect.Height;
        if ((width <= 0) || (height <= 0))
        {
            return;
        }

        const float margin = 0.18f;
        var usable = height * (1f - (margin * 2f));

        // 出力 0 と 1 の高さの補助線
        var bottom = dirtyRect.Bottom - (height * margin);
        var top = bottom - usable;
        canvas.StrokeColor = GuideColor;
        canvas.StrokeSize = 1f;
        canvas.StrokeDashPattern = [4f, 4f];
        canvas.DrawLine(dirtyRect.Left, bottom, dirtyRect.Right, bottom);
        canvas.DrawLine(dirtyRect.Left, top, dirtyRect.Right, top);
        canvas.StrokeDashPattern = null;

        // 丸が左右の端で切れないよう、曲線を丸の半径だけ内側に描く
        var left = dirtyRect.Left + BallRadius;
        var span = width - (BallRadius * 2f);

        using var path = new PathF();
        for (var i = 0; i <= 40; i++)
        {
            var t = i / 40f;
            var v = (float)Easing.Ease(t);
            var x = left + (t * span);
            var y = bottom - (v * usable);
            if (i == 0)
            {
                path.MoveTo(x, y);
            }
            else
            {
                path.LineTo(x, y);
            }
        }

        canvas.StrokeColor = LineColor;
        canvas.StrokeSize = 2f;
        canvas.DrawPath(path);

        var progress = (float)Math.Clamp(Progress, 0d, 1d);
        canvas.FillColor = LineColor;
        canvas.FillCircle(left + (progress * span), bottom - ((float)Easing.Ease(progress) * usable), BallRadius);
    }
}
