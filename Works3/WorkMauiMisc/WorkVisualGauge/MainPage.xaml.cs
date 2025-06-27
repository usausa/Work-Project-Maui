namespace WorkVisualGauge;

using Microsoft.Maui.Graphics;

public partial class MainPage : ContentPage, IDrawable
{
    public int Value { get; set; }

    public double Min { get; set; } = 20;

    public double Max { get; set; } = 120;

    public double Threshold { get; set; } = 100;

    public MainPage()
    {
        InitializeComponent();

        GraphicsView.Drawable = this;

        //_ = RunTimerAsync();
        Value = 45;
    }

    private async Task RunTimerAsync()
    {
        try
        {
            using var timer = new PeriodicTimer(TimeSpan.FromMilliseconds(100));
            while (await timer.WaitForNextTickAsync().ConfigureAwait(true))
            {
                Value += 1;
                if (Value > 100)
                {
                    Value = 0;
                }

                GraphicsView.Invalidate();
            }
        }
        catch (OperationCanceledException)
        {
        }
    }

    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        canvas.FillColor = Colors.Gray;
        canvas.FillRectangle(dirtyRect);

        Draw2(canvas, new RectF(dirtyRect.X, dirtyRect.Y, dirtyRect.Width, dirtyRect.Height / 4));
    }

    // TODO calc2
    public void Draw2(ICanvas canvas, RectF dirtyRect)
    {
        canvas.FillColor = new Color(64, 64, 64);
        canvas.FillRectangle(dirtyRect);
        canvas.Antialias = true;

        var margin = 12;
        var tickSize = 24;

        // dummy
        canvas.FillColor = new Color(96, 96, 96);
        canvas.FillRectangle(dirtyRect.Inflate(-margin, -margin));
        canvas.FillColor = new Color(48, 48, 48);
        canvas.FillRectangle(dirtyRect.Inflate(-(margin + tickSize), -(margin + tickSize)));
        // dummy

        var baseSize = Math.Min((dirtyRect.Width - ((margin + tickSize) * 2)) / 4, (dirtyRect.Height - ((margin + tickSize) * 2)));
        //var baseTop= dirtyRect.Height - margin - baseSize;
        //var baseHeight = dirtyRect.Height - baseTop - margin;
        var baseTop = margin + tickSize;
        var baseHeight = baseSize;

        var sweepRect = new RectF(dirtyRect.X + margin + tickSize, baseTop, dirtyRect.Width - ((margin + tickSize) * 2), baseHeight);

        canvas.FillColor = Colors.Black;
        canvas.FillRectangle(sweepRect);

        // dummy
        canvas.FillColor = new Color(32, 32, 32);
        canvas.FillRectangle(new RectF(sweepRect.X, sweepRect.Y + baseHeight, sweepRect.Width, sweepRect.Height));
        canvas.FillColor = new Color(48, 48, 48);
        canvas.FillRectangle(new RectF(sweepRect.X, sweepRect.Y + (baseHeight * 2), sweepRect.Width, sweepRect.Height));
        canvas.FillRectangle(new RectF(sweepRect.X, sweepRect.Y + (baseHeight * 3), sweepRect.Width, sweepRect.Height));
        // dummy

        var cx = sweepRect.Center.X;
        var cy = sweepRect.Y + (baseHeight * 2);
        var radius = baseHeight * 2;
        var arcRect = new RectF(cx - radius, cy - radius, radius * 2, radius * 2);

        // Tick
        canvas.StrokeLineCap = LineCap.Round;
        for (var i = 0; i <= 100; i += 10)
        {
            var isMajor = i % 20 == 0;
            var angle = (120f * i / 100) - 60;
            var len = isMajor ? tickSize - 12 : tickSize - 16;

            canvas.SaveState();

            canvas.Translate(cx, cy);
            canvas.Rotate(angle);

            canvas.StrokeColor = i >= 80 ? Colors.Red : Colors.White;
            canvas.StrokeSize = isMajor ? 3 : 2;
            canvas.DrawLine(0f, -radius, 0f, -radius - len);

            if (isMajor)
            {
                canvas.FontColor = i >= 80 ? Colors.Red : Colors.White;
                canvas.FontSize = 14;
                canvas.DrawString($"{i}", 0, -radius - len - 4, HorizontalAlignment.Center);
            }

            canvas.ResetState();
        }

        // Arc
        canvas.StrokeSize = 3;
        canvas.StrokeColor = Colors.White;
        canvas.DrawArc(arcRect, 150f, 30f, true, false);

        var redStart = 150f - ((150f - 30f) * 80 / 100);
        canvas.StrokeColor = Colors.Red;
        canvas.DrawArc(arcRect, redStart, 30f, true, false);

        // Needle
        var valueAngle = (120f * Value / 100) - 60;

        canvas.SaveState();

        canvas.ClipRectangle(dirtyRect.Inflate(-margin, -margin));

        canvas.Translate(cx, cy);
        canvas.Rotate(valueAngle);

        canvas.StrokeLineCap = LineCap.Round;
        canvas.StrokeSize = 5;
        canvas.StrokeColor = Colors.OrangeRed;

        canvas.DrawLine(0, 0, 0, -radius - 20);

        canvas.ResetState();
    }
}
