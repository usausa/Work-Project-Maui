using System.Diagnostics;
using System.Timers;

namespace WorkVisualGauge;

using Microsoft.Maui.Graphics;

public partial class MainPage : ContentPage, IDrawable
{
    private const float StartAngle = 210f;
    private const float EndAngle = -30f;
    private const float RangeAngle = StartAngle - EndAngle;

    private double animatedValue;

    private readonly System.Timers.Timer animationTimer = new(1000d / 60);

    // Value

    public double Value { get; set; }

    public double Min { get; set; } = 20;

    public double Max { get; set; } = 120;

    public double Threshold { get; set; } = 80;

    // Size

    public float Margin { get; set; } = 12;

    public float GaugeWidth { get; set; } = 4;

    public float TickLength { get; set; } = 12;

    public float MinorTickLength { get; set; } = 18;

    public float MajorTickLength { get; set; } = 24;

    public float TickWidth { get; set; } = 1;

    public float MinorTickWidth { get; set; } = 2;

    public float MajorTickWidth { get; set; } = 4;

    public float TextOffset { get; set; } = 44;

    public float NeedleWidth { get; set; } = 11;

    public float CenterCircleRadius { get; set; } = 10;

    public float NeedleLengthPercentage { get; set; } = 1f;

    public float NeedleBackLengthPercentage { get; set; } = 0.15f;

    // Font

    public Font Font { get; set; } = new Font("Arial");

    public float FontSize { get; set; } = 18;

    // Color

    public Color TextColor { get; set; } = Colors.White;

    public Color GaugeColor { get; set; } = Colors.White;

    public Color WarningGaugeColor { get; set; } = Colors.Red;

    public Color NeedleColor { get; set; } = Colors.OrangeRed;

    public Color NeedleCenterColor { get; set; } = Colors.Red;

    public MainPage()
    {
        InitializeComponent();

        GraphicsView.Drawable = this;

        // Dummy
        Value = Min;
        //_ = RunTimerAsync();
        //Value = 0;

        animationTimer.Elapsed += TimerOnElapsed;
    }

    // Dummy
    //private async Task RunTimerAsync()
    //{
    //    try
    //    {
    //        using var timer = new PeriodicTimer(TimeSpan.FromMilliseconds(200));
    //        while (await timer.WaitForNextTickAsync().ConfigureAwait(true))
    //        {
    //            Value += 1;
    //            if (Value > 120)
    //            {
    //                Value = 20;
    //            }

    //            GraphicsView.Invalidate();
    //        }
    //    }
    //    catch (OperationCanceledException)
    //    {
    //    }
    //}

    private void TimerOnElapsed(object? sender, ElapsedEventArgs e)
    {
        if (Math.Abs(animatedValue - Value) < 0.1)
        {
            animationTimer.Stop();
            animatedValue = Value;
        }
        else
        {
            var diff = Value - animatedValue;
            animatedValue += diff * 0.1;
        }

        Dispatcher.Dispatch(() => GraphicsView.Invalidate());
    }

    private void Slider_OnValueChanged(object? sender, ValueChangedEventArgs e)
    {
        // [MEMO] In property changed
        animatedValue = Value;
        Value = e.NewValue;
        animationTimer.Start();
    }

    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        var size = Math.Min(dirtyRect.Width, dirtyRect.Height) - Margin * 2;
        var radius = size / 2;
        var rect = new RectF(dirtyRect.Center.X - radius, Margin, size, size);

        var cx = dirtyRect.Center.X;
        var cy = rect.Center.Y;

        var range = Max - Min;

        canvas.Font = Font;
        canvas.FontSize = FontSize;

        // Tick
        canvas.StrokeLineCap = LineCap.Square;
        for (var i = 0; i <= (int)range; i++)
        {
            var angle = (float)((RangeAngle * i / range) - (StartAngle - 90));
            var isMajor = i % 10 == 0;
            var isMinor = i % 5 == 0;

            var width = isMajor ? MajorTickWidth : isMinor ? MinorTickWidth : TickWidth;
            var length = isMajor ? MajorTickLength : isMinor ? MinorTickLength : TickLength;

            canvas.SaveState();

            canvas.Translate(cx, cy);
            canvas.Rotate(angle);

            canvas.StrokeColor = i + Min >= Threshold ? WarningGaugeColor : GaugeColor;
            canvas.StrokeSize = width;
            canvas.DrawLine(0, -radius + 0.5f, 0, -radius + length);

            canvas.RestoreState();

            if (isMajor)
            {
                var textOffset = radius - TextOffset;
                var textRadian = (float)((StartAngle - (RangeAngle * i / range)) * MathF.PI / 180);
                var textX = cx + textOffset * MathF.Cos(textRadian);
                var textY = cy - textOffset * MathF.Sin(textRadian);

                var text = $"{(int)(i + Min)}";
                var textSize = canvas.GetStringSize(text, Font, FontSize);
                canvas.FontColor = TextColor;
                canvas.DrawString(text, textX, textY + (textSize.Height / 2), HorizontalAlignment.Center);
            }
        }

        // Arc
        canvas.StrokeSize = GaugeWidth;
        canvas.StrokeColor = GaugeColor;
        canvas.DrawArc(rect, StartAngle, EndAngle, true, false);

        var warningStart = StartAngle - (float)(RangeAngle * (Threshold - Min) / range);
        canvas.StrokeColor = WarningGaugeColor;
        canvas.DrawArc(rect, warningStart, EndAngle, true, false);

        // Needle
        var needleRadian = (float)((StartAngle - (RangeAngle * (animatedValue - Min) / range)) * MathF.PI / 180);

        var needleLength = (radius - MajorTickLength) * NeedleLengthPercentage;
        var needleBackLength = (radius - MajorTickLength) * NeedleBackLengthPercentage;
        var baseCenterX = cx - (needleBackLength * MathF.Cos(needleRadian));
        var baseCenterY = cy + (needleBackLength * MathF.Sin(needleRadian));
        var baseAngle1 = needleRadian + (MathF.PI / 2);
        var baseAngle2 = needleRadian - (MathF.PI / 2);
        var baseRadius = NeedleWidth / 2;

        var path = new PathF();
        path.MoveTo(cx + needleLength * MathF.Cos(needleRadian), cy - needleLength * MathF.Sin(needleRadian));
        path.LineTo(baseCenterX + (baseRadius * MathF.Cos(baseAngle1)), baseCenterY - (baseRadius * MathF.Sin(baseAngle1)));
        path.LineTo(baseCenterX + (baseRadius * MathF.Cos(baseAngle2)), baseCenterY - (baseRadius * MathF.Sin(baseAngle2)));
        path.Close();

        canvas.FillColor = NeedleColor;
        canvas.FillPath(path);

        // Center
        var centerCircleRadius = CenterCircleRadius;
        canvas.FillColor = NeedleCenterColor;
        canvas.FillCircle(cx, cy, centerCircleRadius);
    }
}
