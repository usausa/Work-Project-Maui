namespace Template.MobileApp.Graphics;

using System.Timers;

public enum ChartKind
{
    Line,
    Bar,
    Donut,
    Candle
}

public readonly record struct CandlePoint(double Open, double High, double Low, double Close);

public sealed class ChartGraphics : GraphicsObject, IDisposable
{
    private const float AnimationDuration = 600f;
    private const float Padding = 24f;

    private static readonly Color BackgroundColor = Colors.White;
    private static readonly Color AxisColor = Color.FromArgb("#E0E0E0");
    private static readonly Color TextColor = Color.FromArgb("#757575");
    private static readonly Color LineColor = Color.FromArgb("#2196F3");
    private static readonly Color BarColor = Color.FromArgb("#42A5F5");
    private static readonly Color UpColor = Color.FromArgb("#43A047");
    private static readonly Color DownColor = Color.FromArgb("#E53935");

    private static readonly Color[] DonutColors =
    [
        Color.FromArgb("#2196F3"),
        Color.FromArgb("#4CAF50"),
        Color.FromArgb("#FFB300"),
        Color.FromArgb("#EC407A"),
        Color.FromArgb("#26C6DA"),
    ];

    private readonly System.Timers.Timer animationTimer = new(1000d / 60);

    private ChartKind kind = ChartKind.Line;

    private IReadOnlyList<double> values = [];

    private IReadOnlyList<CandlePoint> candles = [];

    private float progress = 1f;

    private long animationStart;

    public ChartGraphics()
    {
        animationTimer.Elapsed += TimerElapsed;
    }

    public void Dispose()
    {
        animationTimer.Dispose();
    }

    public void ShowLine(IReadOnlyList<double> data)
    {
        kind = ChartKind.Line;
        values = data;
        StartAnimation();
    }

    public void ShowBar(IReadOnlyList<double> data)
    {
        kind = ChartKind.Bar;
        values = data;
        StartAnimation();
    }

    public void ShowDonut(IReadOnlyList<double> data)
    {
        kind = ChartKind.Donut;
        values = data;
        StartAnimation();
    }

    public void ShowCandle(IReadOnlyList<CandlePoint> data)
    {
        kind = ChartKind.Candle;
        candles = data;
        StartAnimation();
    }

    private void StartAnimation()
    {
        progress = 0f;
        animationStart = Environment.TickCount64;
        animationTimer.Start();
        SafeInvalidate();
    }

    private void TimerElapsed(object? sender, ElapsedEventArgs e)
    {
        var elapsed = Environment.TickCount64 - animationStart;
        progress = Math.Min(1f, elapsed / AnimationDuration);
        if (progress >= 1f)
        {
            animationTimer.Stop();
        }

        SafeInvalidate();
    }

    protected override void OnDraw(ICanvas canvas, RectF dirtyRect)
    {
        canvas.SaveState();
        canvas.FillColor = BackgroundColor;
        canvas.FillRectangle(dirtyRect);
        canvas.Antialias = true;

        // CubicOut
        var t = progress;
        var eased = 1f - ((1f - t) * (1f - t) * (1f - t));

        switch (kind)
        {
            case ChartKind.Line:
                DrawLineChart(canvas, dirtyRect, eased);
                break;
            case ChartKind.Bar:
                DrawBarChart(canvas, dirtyRect, eased);
                break;
            case ChartKind.Donut:
                DrawDonutChart(canvas, dirtyRect, eased);
                break;
            default:
                DrawCandleChart(canvas, dirtyRect, eased);
                break;
        }

        canvas.RestoreState();
    }

    //--------------------------------------------------------------------------------
    // Area
    //--------------------------------------------------------------------------------

    private static RectF GetPlotArea(RectF dirtyRect) =>
        new(dirtyRect.Left + Padding, dirtyRect.Top + Padding, dirtyRect.Width - (Padding * 2), dirtyRect.Height - (Padding * 2));

    //--------------------------------------------------------------------------------
    // Line
    //--------------------------------------------------------------------------------

    private void DrawLineChart(ICanvas canvas, RectF dirtyRect, float eased)
    {
        var area = GetPlotArea(dirtyRect);
        DrawGrid(canvas, area);

        if (values.Count < 2)
        {
            return;
        }

        var min = values.Min();
        var max = values.Max();
        var range = Math.Max(1e-6, max - min);

        PointF GetPoint(int i) =>
            new(
                area.Left + (area.Width * i / (values.Count - 1)),
                area.Bottom - (float)((values[i] - min) / range * area.Height));

        // 左から右へ描画をクリップして伸ばす
        canvas.SaveState();
        canvas.ClipRectangle(area.Left, dirtyRect.Top, area.Width * eased, dirtyRect.Height);

        // 線下のグラデーション
        using (var fillPath = new PathF())
        {
            fillPath.MoveTo(area.Left, area.Bottom);
            for (var i = 0; i < values.Count; i++)
            {
                fillPath.LineTo(GetPoint(i));
            }
            fillPath.LineTo(area.Right, area.Bottom);
            fillPath.Close();

            var gradient = new LinearGradientPaint(
                [
                    new PaintGradientStop(0f, LineColor.WithAlpha(0.30f)),
                    new PaintGradientStop(1f, LineColor.WithAlpha(0.02f))
                ],
                startPoint: new Point(0, 0),
                endPoint: new Point(0, 1));
            canvas.SetFillPaint(gradient, area);
            canvas.FillPath(fillPath);
        }

        // 折れ線
        using (var linePath = new PathF())
        {
            linePath.MoveTo(GetPoint(0));
            for (var i = 1; i < values.Count; i++)
            {
                linePath.LineTo(GetPoint(i));
            }

            canvas.StrokeColor = LineColor;
            canvas.StrokeSize = 3f;
            canvas.StrokeLineJoin = LineJoin.Round;
            canvas.StrokeLineCap = LineCap.Round;
            canvas.DrawPath(linePath);
        }

        // 点
        canvas.FillColor = Colors.White;
        canvas.StrokeColor = LineColor;
        canvas.StrokeSize = 2f;
        for (var i = 0; i < values.Count; i++)
        {
            var p = GetPoint(i);
            canvas.FillCircle(p.X, p.Y, 4f);
            canvas.DrawCircle(p.X, p.Y, 4f);
        }

        canvas.RestoreState();
    }

    //--------------------------------------------------------------------------------
    // Bar
    //--------------------------------------------------------------------------------

    private void DrawBarChart(ICanvas canvas, RectF dirtyRect, float eased)
    {
        var area = GetPlotArea(dirtyRect);
        DrawGrid(canvas, area);

        if (values.Count == 0)
        {
            return;
        }

        var max = Math.Max(1e-6, values.Max());
        var barWidth = area.Width / values.Count * 0.6f;
        var step = area.Width / values.Count;

        canvas.FillColor = BarColor;
        for (var i = 0; i < values.Count; i++)
        {
            var height = (float)(values[i] / max * area.Height) * eased;
            var x = area.Left + (step * i) + ((step - barWidth) / 2f);
            canvas.FillRoundedRectangle(x, area.Bottom - height, barWidth, height, 4f);
        }
    }

    //--------------------------------------------------------------------------------
    // Donut
    //--------------------------------------------------------------------------------

    private void DrawDonutChart(ICanvas canvas, RectF dirtyRect, float eased)
    {
        if (values.Count == 0)
        {
            return;
        }

        var total = values.Sum();
        if (total <= 0)
        {
            return;
        }

        var cx = dirtyRect.Center.X;
        var cy = dirtyRect.Center.Y;
        var radius = (Math.Min(dirtyRect.Width, dirtyRect.Height) / 2f) - Padding - 16f;
        const float thickness = 32f;
        var rect = new RectF(cx - radius, cy - radius, radius * 2, radius * 2);

        canvas.StrokeSize = thickness;

        // 上(90 度)から時計回りに全体の eased 分までスイープ
        var startAngle = 90f;
        var remaining = 360f * eased;
        for (var i = 0; i < values.Count; i++)
        {
            var sweep = (float)(values[i] / total * 360f);
            var draw = Math.Min(sweep, remaining);
            if (draw <= 0f)
            {
                break;
            }

            canvas.StrokeColor = DonutColors[i % DonutColors.Length];
            canvas.DrawArc(rect, startAngle, startAngle - draw, true, false);
            startAngle -= sweep;
            remaining -= draw;
        }

        // 中央の合計値
        canvas.FontSize = 26f;
        canvas.FontColor = Color.FromArgb("#424242");
        canvas.DrawString($"{total * eased:N0}", cx, cy - 2f, HorizontalAlignment.Center);
        canvas.FontSize = 12f;
        canvas.FontColor = TextColor;
        canvas.DrawString("TOTAL", cx, cy + 18f, HorizontalAlignment.Center);
    }

    //--------------------------------------------------------------------------------
    // Candle
    //--------------------------------------------------------------------------------

    private void DrawCandleChart(ICanvas canvas, RectF dirtyRect, float eased)
    {
        var area = GetPlotArea(dirtyRect);
        DrawGrid(canvas, area);

        if (candles.Count == 0)
        {
            return;
        }

        var min = candles.Min(static x => x.Low);
        var max = candles.Max(static x => x.High);
        var range = Math.Max(1e-6, max - min);

        float ToY(double value) => area.Bottom - (float)((value - min) / range * area.Height);

        var step = area.Width / candles.Count;
        var bodyWidth = step * 0.55f;

        // 左から順に出現させる
        var visible = (int)Math.Ceiling(candles.Count * eased);
        for (var i = 0; i < visible; i++)
        {
            var candle = candles[i];
            var cx = area.Left + (step * i) + (step / 2f);
            var color = candle.Close >= candle.Open ? UpColor : DownColor;

            canvas.StrokeColor = color;
            canvas.StrokeSize = 1.5f;
            canvas.DrawLine(cx, ToY(candle.High), cx, ToY(candle.Low));

            var top = ToY(Math.Max(candle.Open, candle.Close));
            var bottom = ToY(Math.Min(candle.Open, candle.Close));
            canvas.FillColor = color;
            canvas.FillRoundedRectangle(cx - (bodyWidth / 2f), top, bodyWidth, Math.Max(2f, bottom - top), 2f);
        }
    }

    private static void DrawGrid(ICanvas canvas, RectF area)
    {
        canvas.StrokeColor = AxisColor;
        canvas.StrokeSize = 1f;
        for (var i = 0; i <= 4; i++)
        {
            var y = area.Top + (area.Height * i / 4f);
            canvas.DrawLine(area.Left, y, area.Right, y);
        }
    }
}
