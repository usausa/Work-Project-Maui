namespace Template.MobileApp.Graphics.Drawing;

public sealed class LoadDrawing : DrawingObject
{
    private const int MaxBars = 60;

    private const float Gap = 2f;

    private const float AxisWidth = 30f;

    private const float VerticalPadding = 8f;

    private const int ScaleStep = 20;

    private static readonly Color BackgroundColor = Color.FromArgb("#2E2F45");

    private static readonly Color GridColor = Color.FromRgba(255, 255, 255, 28);

    private static readonly Color ScaleTextColor = Color.FromArgb("#9FA3C3");

    // 目安の線(DecibelToColorConverter と同じ 50 / 70 dB)。線の上の帯の名前と、50 の線の下に QUIET
    private static readonly (float Value, string Label, Color Color)[] Guides =
    [
        (50f, "NORMAL", Colors.Gold),
        (70f, "LOUD", Colors.Red)
    ];

    private static readonly LinearGradientPaint GradientPaint = new(
        [
            new PaintGradientStop(0f, Colors.LimeGreen),
            new PaintGradientStop(0.5f, Colors.Gold),
            new PaintGradientStop(1f, Colors.Red)
        ],
        startPoint: new Point(0, 1),
        endPoint: new Point(0, 0));

    private readonly float[] buffer = new float[MaxBars];

    private int writeIndex;

    private int count;

    public int Min
    {
        get;
        set
        {
            field = value;
            Invalidate();
        }
    }

    public int Max
    {
        get;
        set
        {
            field = value;
            Invalidate();
        }
    }

    public LoadDrawing()
    {
        Min = 0;
        Max = 100;
    }

    public (double Avg, double Min, double Max) CalcStatics()
    {
        if (count == 0)
        {
            return (0, 0, 0);
        }

        var sum = 0d;
        var min = double.MaxValue;
        var max = double.MinValue;
        for (var i = 0; i < count; i++)
        {
            var index = (writeIndex - 1 - i + MaxBars) % MaxBars;
            var value = buffer[index];
            sum += value;
            if (value < min)
            {
                min = value;
            }
            if (value > max)
            {
                max = value;
            }
        }

        return (sum / count, min, max);
    }

    public void Clear()
    {
        count = 0;
        Invalidate();
    }

    public void AddValue(float value)
    {
        buffer[writeIndex] = value;
        writeIndex = (writeIndex + 1) % MaxBars;
        if (count < MaxBars)
        {
            count++;
        }

        Invalidate();
    }

    protected override void OnDraw(ICanvas canvas, RectF dirtyRect)
    {
        canvas.FillColor = BackgroundColor;
        canvas.FillRectangle(dirtyRect);

        var range = (float)(Max - Min);
        if (range <= 0)
        {
            return;
        }

        var plot = new RectF(dirtyRect.Left + AxisWidth, dirtyRect.Top + VerticalPadding, dirtyRect.Width - AxisWidth - Gap, dirtyRect.Height - (VerticalPadding * 2));
        DrawBars(canvas, plot, range);
        DrawScale(canvas, dirtyRect, plot, range);
    }

    private float ToY(RectF plot, float range, float value) =>
        plot.Bottom - (plot.Height * Math.Clamp((value - Min) / range, 0f, 1f));

    private void DrawBars(ICanvas canvas, RectF plot, float range)
    {
        var barWidth = plot.Width / MaxBars;
        for (var i = 0; i < count; i++)
        {
            var idx = (writeIndex - 1 - i + MaxBars) % MaxBars;
            var x = plot.Right - ((i + 1) * barWidth);
            var fullRect = new RectF(x + (Gap / 2), plot.Top, barWidth - Gap, plot.Height);

            canvas.SaveState();
            canvas.SetFillPaint(GradientPaint, fullRect);
            canvas.FillRectangle(fullRect);
            canvas.RestoreState();

            // 値より上を地の色で隠す
            var maskRect = new RectF(x, plot.Top, barWidth, ToY(plot, range, buffer[idx]) - plot.Top);
            canvas.FillColor = BackgroundColor;
            canvas.FillRectangle(maskRect);
        }
    }

    private void DrawScale(ICanvas canvas, RectF area, RectF plot, float range)
    {
        canvas.StrokeSize = 1;
        canvas.FontSize = 10f;
        for (var value = Min; value <= Max; value += ScaleStep)
        {
            var y = ToY(plot, range, value);
            canvas.StrokeColor = GridColor;
            canvas.DrawLine(plot.Left, y, plot.Right, y);
            canvas.FontColor = ScaleTextColor;
            canvas.DrawString(value.ToString(CultureInfo.InvariantCulture), area.Left, y - 7f, AxisWidth - 6f, 14f, HorizontalAlignment.Right, VerticalAlignment.Center);
        }

        canvas.StrokeDashPattern = [4f, 3f];
        foreach (var (value, label, color) in Guides)
        {
            var y = ToY(plot, range, value);
            canvas.StrokeColor = color.WithAlpha(0.85f);
            canvas.DrawLine(plot.Left, y, plot.Right, y);
            canvas.FontColor = color;
            canvas.DrawString(label, plot.Left + 4f, y - 15f, 80f, 13f, HorizontalAlignment.Left, VerticalAlignment.Bottom);
        }

        canvas.StrokeDashPattern = null;
        canvas.FontColor = Colors.LimeGreen;
        canvas.DrawString("QUIET", plot.Left + 4f, ToY(plot, range, Guides[0].Value) + 2f, 80f, 13f, HorizontalAlignment.Left, VerticalAlignment.Top);
    }
}
