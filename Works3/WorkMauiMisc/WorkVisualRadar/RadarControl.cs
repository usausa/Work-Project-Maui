namespace WorkVisualRadar;

#pragma warning disable CA1001
public sealed class RadarControl : GraphicsView, IDrawable
{
    // スイープの幅（度数法）
    private const float SweepWidth = 60f;

    private static readonly TimeSpan Interval = TimeSpan.FromMilliseconds(1000d / 60);

    private CancellationTokenSource? cts;

    // 現在のスイープ開始角度（度数法）
    private float currentAngle;

    public float CurrentAngle
    {
        get => currentAngle;
        set => currentAngle = value;
    }

    public RadarControl()
    {
        Drawable = this;
    }

    protected override void OnHandlerChanged()
    {
        base.OnHandlerChanged();

        if (Handler != null)
        {
            StartTimer();
        }
        else
        {
            StopTimer();
        }
    }

    private void StartTimer()
    {
        if ((cts is not null) && !cts.IsCancellationRequested)
        {
            return;
        }

        cts = new CancellationTokenSource();
        _ = Loop(cts.Token);
    }

    private void StopTimer()
    {
        if (cts is null)
        {
            return;
        }

        cts.Cancel();
        cts.Dispose();
        cts = null;
    }

    private async Task Loop(CancellationToken ct)
    {
        try
        {
            using var timer = new PeriodicTimer(Interval);
            while (await timer.WaitForNextTickAsync(ct))
            {
                if (ct.IsCancellationRequested)
                {
                    break;
                }

                MainThread.BeginInvokeOnMainThread(() =>
                {
                    currentAngle += 2f;
                    if (currentAngle >= 360f)
                    {
                        currentAngle -= 360f;
                    }

                    Invalidate();
                });
            }
        }
        catch (OperationCanceledException)
        {
            // Ignore
        }
    }

    private static float DegreesToRadians(float degrees) =>
        degrees * ((float)Math.PI / 180f);

    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        var width = dirtyRect.Width;
        var height = dirtyRect.Height;

        // 中心座標
        var cx = width / 2f;
        var cy = height / 2f;

        // 半径（外枠はキャンバスの最小辺の90%）
        var radius = Math.Min(cx, cy) * 0.9f;

        // 背景クリア
        canvas.FillColor = Colors.Black;
        canvas.FillRectangle(0, 0, width, height);

        // 線の共通設定
        canvas.StrokeColor = Colors.Green;
        canvas.StrokeSize = 2;

        // 外枠の円
        canvas.DrawEllipse(cx - radius, cy - radius, radius * 2, radius * 2);

        // 同心円
        for (var i = 1; i <= 3; i++)
        {
            var r = radius * i / 3f;
            canvas.DrawEllipse(cx - r, cy - r, r * 2, r * 2);
        }

        // 斜め十字線（45°ごとに放射線）
        for (var a = 0; a < 360; a += 45)
        {
            var rad = DegreesToRadians(a);
            var x = cx + radius * (float)Math.Cos(rad);
            var y = cy + radius * (float)Math.Sin(rad);
            canvas.DrawLine(cx, cy, x, y);
        }

        // 目盛線 (5°ごと、小さい／10°ごと、大きい)
        for (var a = 0; a < 360; a += 5)
        {
            var rad = DegreesToRadians(a);
            var outer = radius;
            var inner = radius - (a % 10 == 0 ? 15 : 8);
            var x1 = cx + outer * (float)Math.Cos(rad);
            var y1 = cy + outer * (float)Math.Sin(rad);
            var x2 = cx + inner * (float)Math.Cos(rad);
            var y2 = cy + inner * (float)Math.Sin(rad);
            canvas.DrawLine(x1, y1, x2, y2);
        }

        //// スイープエリア (先端が濃く、後ろが薄いグラデーション)
        //var sweepSteps = 30;
        //var stepAngle = SweepWidth / sweepSteps;
        //for (var i = 0; i < sweepSteps; i++)
        //{
        //    // 各スライスの開始角度
        //    var angle = currentAngle - SweepWidth / 2f + stepAngle * i;

        //    // α: 後ろが薄く（20）、先端が濃く（255）
        //    var alpha = 20f + (235f * (i + 1) / sweepSteps);

        //    canvas.FillColor = new Color(0, 255, 0, alpha / 255f);
        //    // center フラグを true にすると扇形 (Pie) 描画になる
        //    canvas.FillArc(
        //        cx - radius,
        //        cy - radius,
        //        radius * 2,
        //        radius * 2,
        //        angle,
        //        stepAngle,
        //        true);
        //}

        // スイープ先端ライン（弧の最後の部分）
        canvas.StrokeColor = Colors.Lime;
        canvas.StrokeSize = 3;

        var endAngle = currentAngle + SweepWidth / 2f;
        var radEnd = DegreesToRadians(endAngle);
        var ex = cx + radius * (float)Math.Cos(radEnd);
        var ey = cy + radius * (float)Math.Sin(radEnd);
        canvas.DrawLine(cx, cy, ex, ey);
    }
}
#pragma warning restore CA1001
