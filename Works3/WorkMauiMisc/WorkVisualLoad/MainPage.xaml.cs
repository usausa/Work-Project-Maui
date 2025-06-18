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
        // TODO scaleも
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
            ],
            startPoint: new Point(0, 1),   // 下端が green
            endPoint: new Point(0, 0));    // 上端が red


        for (int i = 0; i < count; i++)
        {
            int idx = (writeIndex - 1 - i + MaxBars) % MaxBars;
            float value = buffer[idx];
            if (value <= 0f)
                continue;

            float x = dirtyRect.Right - (i + 1) * barWidth;
            float fullHeight = dirtyRect.Height;
            float yTop = dirtyRect.Top;
            var fullRect = new RectF(x + (gap / 2), yTop, barWidth - gap, fullHeight);

            canvas.SaveState();
            // 1) full-height でグラデーション描画
            canvas.SetFillPaint(gradientPaint, fullRect);
            canvas.FillRectangle(fullRect);

            // 2) 上側 (1 - value) 部分を黒で塗りつぶし
            float maskHeight = fullHeight * (1f - value);
            var maskRect = new RectF(x, yTop, barWidth, maskHeight);
            canvas.FillColor = Colors.Black;
            canvas.FillRectangle(maskRect);

            canvas.RestoreState();
        }
    }
}
