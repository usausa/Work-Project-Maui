namespace Template.MobileApp.Graphics.Drawing;

using System.Timers;

// 受けた値の線を右から左へ滑らかに流す。先頭 (最新の値) は右の端の点で、古い側ほど薄くする。
// 値を Add すると Interval の時間で 1 区間ぶん流れ、次の値が来るまでは止まる
public sealed class FlowLineDrawing : DrawingObject, IDisposable
{
    private readonly Timer timer = new(1000d / 60);

    private readonly List<double> values = [];

    private long addedAt;

    private float phase = 1f;

    // 線と先頭の点の色
    public Color LineColor { get; set; } = Color.FromArgb("#2196F3");

    public float LineWidth { get; set; } = 2.5f;

    // 横に並べる区間の数
    public int Capacity { get; set; } = 30;

    // 値が届く間隔 (ms)。この時間で 1 区間流れる
    public double Interval { get; set; } = 1000;

    // 縦の範囲 (Maximum が Minimum 以下なら値の最小と最大)
    public double Minimum { get; set; }

    public double Maximum { get; set; }

    public FlowLineDrawing()
    {
        timer.Elapsed += TimerElapsed;
    }

    public void Dispose()
    {
        timer.Dispose();
    }

    public void Add(double value)
    {
        values.Add(value);
        if (values.Count > Capacity + 2)
        {
            values.RemoveAt(0);
        }

        addedAt = Environment.TickCount64;
        phase = 0f;
        timer.Start();
        SafeInvalidate();
    }

    public void Clear()
    {
        values.Clear();
        timer.Stop();
        phase = 1f;
        SafeInvalidate();
    }

    private void TimerElapsed(object? sender, ElapsedEventArgs e)
    {
        phase = Math.Min(1f, (float)((Environment.TickCount64 - addedAt) / Interval));
        if (phase >= 1f)
        {
            timer.Stop();
        }

        SafeInvalidate();
    }

    protected override void OnDraw(ICanvas canvas, RectF dirtyRect)
    {
        if (values.Count < 2)
        {
            return;
        }

        var min = Minimum;
        var max = Maximum;
        if (max <= min)
        {
            min = values.Min();
            max = values.Max();
            if ((max - min) < 1e-6)
            {
                min -= 1;
                max += 1;
            }
        }

        // 先頭の点が切れないよう、右と上下に点の分の余白を取る
        var dot = LineWidth * 2;
        var left = dirtyRect.Left;
        var right = dirtyRect.Right - dot;
        var top = dirtyRect.Top + dot;
        var bottom = dirtyRect.Bottom - dot;
        var step = (right - left) / Capacity;

        // 最新の値は区間の途中では右の端の外にあり、phase が 1 になると右の端に来る
        var count = values.Count;
        float X(int i) => right - ((count - 1 - i) * step) + ((1 - phase) * step);
        float Y(int i) => (float)(bottom - ((values[i] - min) / (max - min) * (bottom - top)));

        canvas.SaveState();
        canvas.Antialias = true;
        canvas.ClipRectangle(left, dirtyRect.Top, right - left, dirtyRect.Height);
        canvas.StrokeSize = LineWidth;
        canvas.StrokeLineCap = LineCap.Round;
        for (var i = 0; i < count - 1; i++)
        {
            var x1 = X(i);
            var x2 = X(i + 1);
            if (x2 < left)
            {
                continue;
            }

            // 左へ行くほど薄くする
            var alpha = Math.Clamp((((x1 + x2) / 2) - left) / (right - left), 0.08f, 1f);
            canvas.StrokeColor = LineColor.WithAlpha(alpha);
            canvas.DrawLine(x1, Y(i), x2, Y(i + 1));
        }

        canvas.RestoreState();

        // 先頭の点は、右の端での線の高さに置く
        var y = Y(count - 2) + ((Y(count - 1) - Y(count - 2)) * phase);
        canvas.FillColor = LineColor.WithAlpha(0.25f);
        canvas.FillCircle(right, y, dot * 1.6f);
        canvas.FillColor = LineColor;
        canvas.FillCircle(right, y, dot);
    }
}
