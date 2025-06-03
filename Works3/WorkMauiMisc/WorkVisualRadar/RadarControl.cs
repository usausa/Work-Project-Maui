namespace WorkVisualRadar;

using Microsoft.Maui.Graphics;

using System;

#pragma warning disable CA1001
public sealed class RadarControl : GraphicsView, IDrawable
{
    // スイープの幅（度数法）
    private const float SweepLength = 60f;

    private static readonly TimeSpan Interval = TimeSpan.FromMilliseconds(1000d / 60);

    private CancellationTokenSource? cts;

    // 現在のスイープ開始角度（度数法）
    private float currentAngle = 90;

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

                //MainThread.BeginInvokeOnMainThread(() =>
                //{
                //    currentAngle += 2f;
                //    if (currentAngle >= 360f)
                //    {
                //        currentAngle -= 360f;
                //    }

                //    Invalidate();
                //});
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

        // 背景クリア
        canvas.FillColor = Colors.Black;
        canvas.FillRectangle(0, 0, width, height);

        var margin = 5;
        var cx = width / 2;
        var cy = height / 2;
        var radius = Math.Min(cx, cy) - margin;

        var isClockWise = true;
        var progress = 60f;
        var progressAngle = progress / 100;

        var startAngle = 90f;
        var endAngle = startAngle - (progressAngle * 360f);

        var path = new PathF();
        path.MoveTo(cx, cy);
        path.AddArc(cx - radius, cy - radius, cx + radius, cy + radius, startAngle, endAngle, isClockWise);
        path.Close();

        canvas.FillColor = new Color(0, 255, 0, 64);
        canvas.FillPath(path);
    }

    public void Draw2(ICanvas canvas, RectF dirtyRect)
    {
        var width = dirtyRect.Width;
        var height = dirtyRect.Height;

        // 中心座標
        var cx = width / 2f;
        var cy = height / 2f;
        // 半径（外枠はキャンバスの最小辺の90%）
        var radius = Math.Min(cx, cy) * 0.9f;

        canvas.Antialias = true;

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
        // --------------------------------------------------

        //// スイープエリア(外周に沿った円弧をグラデーションで描画)
        //float stepAngle = 1;
        //// アークはすべて同じ半径、中心(cx,cy)を使って描画
        //for (int i = 0; i < 3; i++)
        //{
        //    float start = currentAngle + stepAngle * i;
        //    // α: 後ろが薄く（20）、先端が濃く（255）
        //    float alpha = 20f + (235f * (i + 1) / 30);

        //    canvas.FillColor = new Color(0f, 255, 0f, 64);
        //    //canvas.StrokeSize = 2;
        //    var path = new PathF();
        //    path.MoveTo(cx, cy);
        //    path.AddArc(
        //        cx - radius,
        //        cy - radius,
        //        cx + radius,
        //        cy + radius,
        //        start,
        //        stepAngle,
        //        false);
        //    path.LineTo(cx, cy);
        //    path.Close();

        //    canvas.DrawPath(path);

        //    // 円弧だけ描画（中心線は含めない）
        //    //canvas.DrawArc(
        //    //    cx - radius,
        //    //    cy - radius,
        //    //    radius * 2,
        //    //    radius * 2,
        //    //    0,
        //    //    350,
        //    //    true,
        //    //    true);
        //}

        // スイープ尾を Path で描画 (扇形)
        var startAngle = (currentAngle + SweepLength) % 360f;
        var path = new PathF();
        path.MoveTo(cx, cy);
        path.AddArc(cx - radius, cy - radius, radius * 2, radius * 2, startAngle, currentAngle, true);
        path.Close();

        canvas.FillColor = new Color(0, 255, 0, 64);
        canvas.FillPath(path);

        //float startAngle = (currentAngle - SweepLength + 360f) % 360f;
        //var circleBounds = new RectF(cx - radius, cy - radius, radius * 2, radius * 2);
        //var path = new PathF();
        // 中心→尾の始点
        //path.MoveTo(cx, cy);
        //path.LineTo(ex, ey);
        // 扇形の外周 arc
        //path.AddArc(cx - radius, cy - radius, radius * 2, radius * 2, startAngle, startAngle + 30, false);
        //path.LineTo(cx, cy);
        //path.AddArc(cx, cy, radius * 2, radius * 2, startAngle, currentAngle, true);
        //path.AddArc(cx - radius, cy - radius, radius * 2, radius * 2, 45, 0, true);
        //path.AddArc(circleBounds, startAngle, SweepLength);
        // 閉じる
        //path.Close();
        // グラデーションブラシ
        //var stops = new GradientStopCollection
        //{
        //    new GradientStop(Colors.Green.WithAlpha(0f), 0f),
        //    new GradientStop(Colors.Lime.WithAlpha(0.7f), 1f)
        //};
        // 線の方向に合わせて水平グラデーション
        //ar brush = new LinearGradientBrush(stops, new Point(0, 0), new Point(1, 0));

        //canvas.SetFillPaint(brush, circleBounds);
        //canvas.FillPath(path);

        //canvas.FillArc(circleBounds, 45, 345, true);
        //canvas.FillArc(cx - radius, cy - radius, radius * 2, radius * 2, 345, 45, false);
        //canvas.StrokeColor = Colors.Red;
        //canvas.DrawArc(cx - radius, cy - radius, radius * 2, radius * 2, 345, 45, false, false);

        // --------------------------------------------------
        // スイープ先端ライン（弧の最後の部分）
        canvas.StrokeColor = Colors.Lime;
        canvas.StrokeSize = 3;

        var radEnd = DegreesToRadians(currentAngle);
        var ex = cx + radius * (float)Math.Cos(radEnd);
        var ey = cy + radius * (float)Math.Sin(radEnd);

        canvas.DrawLine(cx, cy, ex, ey);
    }
}
#pragma warning restore CA1001
