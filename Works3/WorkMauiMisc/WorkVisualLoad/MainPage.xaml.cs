namespace WorkVisualLoad;

public partial class MainPage : ContentPage, IDrawable
{
    private const int MaxBars = 60;
    private readonly float[] buffer = new float[MaxBars];
    private int writeIndex;
    private int count;

    public MainPage()
    {
        InitializeComponent();

        GraphicsView.Drawable = this;

        _ = RunTimerAsync();
    }

    private async Task RunTimerAsync()
    {
        try
        {
            var random = new Random();

            using var timer = new PeriodicTimer(TimeSpan.FromMilliseconds(500));
            while (await timer.WaitForNextTickAsync())
            {
                AddValue(random.NextSingle());
                GraphicsView.Invalidate();
            }
        }
        catch (OperationCanceledException)
        {
        }
    }

    public void AddValue(float v)
    {
        buffer[writeIndex] = v;
        writeIndex = (writeIndex + 1) % MaxBars;
        if (count < MaxBars)
        {
            count++;
        }
    }

    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        canvas.FillColor = Colors.Black;
        canvas.FillRectangle(dirtyRect);

        if (count == 0)
        {
            return;
        }

        // バー幅と隙間
        var barWidth = dirtyRect.Width / MaxBars;
        var gap = 2f;

        var gradientPaint = new LinearGradientPaint(
            [
                new PaintGradientStop(0f, Colors.LimeGreen),
                new PaintGradientStop(0.5f, Colors.Gold),
                new PaintGradientStop(1f, Colors.Red)
                //new PaintGradientStop(0f, new Color(0x69, 0xf0, 0xae)),
                //new PaintGradientStop(0.5f, new Color(0xff, 0xab, 0x40)),
                //new PaintGradientStop(1f, new Color(0xff, 0x52, 0x52))
            ],
            startPoint: new Point(0, 1),   // 下端が green
            endPoint: new Point(0, 0));    // 上端が red

        for (var i = 0; i < count; i++)
        {
            var idx = (writeIndex - 1 - i + MaxBars) % MaxBars;
            var value = buffer[idx];
            if (value <= 0f)
                continue;

            var x = dirtyRect.Right - (i + 1) * barWidth;
            var fullHeight = dirtyRect.Height;
            var yTop = dirtyRect.Top;
            var fullRect = new RectF(x + (gap / 2), yTop, barWidth - gap, fullHeight);

            canvas.SaveState();
            // 1) full-height でグラデーション描画
            canvas.SetFillPaint(gradientPaint, fullRect);
            canvas.FillRectangle(fullRect);
            canvas.RestoreState();

            // 2) 上側 (1 - value) 部分を黒で塗りつぶし
            var maskHeight = fullHeight * (1f - value);
            var maskRect = new RectF(x, yTop, barWidth, maskHeight);
            canvas.FillColor = Colors.Black;
            canvas.FillRectangle(maskRect);
        }
    }
}
