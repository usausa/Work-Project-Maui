namespace WorkVisualGauge;

using Microsoft.Maui.Graphics;

public partial class MainPage : ContentPage, IDrawable
{
    public double Value { get; set; } = 50;

    public double Min { get; set; } = 20;

    public double Max { get; set; } = 120;

    public double Threshold { get; set; } = 100;

    public MainPage()
    {
        InitializeComponent();

        GraphicsView.Drawable = this;
    }

    const double MajorStep = 20;
    const double MinorStep = 10;
    const float MajorTickLength = 20f;
    const float MinorTickLength = 10f;
    const float LabelOffset = 30f;

    const double StartAngle = 150;   // degrees
    const double EndAngle = 30;     // degrees

    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        canvas.SaveState();

        float cx = dirtyRect.Center.X;
        float cy = dirtyRect.Bottom;
        float radius = Math.Min(dirtyRect.Width, dirtyRect.Height) * 0.45f;

        // draw gauge arc (top half)
        canvas.StrokeColor = Colors.White;
        canvas.StrokeSize = 2;

        var arcRect = new RectF(cx - radius, cy - radius, radius * 2, radius * 2);
        float sweep = (float)(StartAngle - EndAngle); // negative ⇒ clockwise half-circle
        canvas.DrawArc(arcRect, (float)StartAngle, (float)EndAngle, true, false);

        //// draw ticks and labels
        //for (double v = Min; v <= Max + 0.001; v += MinorStep)
        //{
        //    bool isMajor = Math.Abs(v % MajorStep) < 0.001;
        //    float len = isMajor ? MajorTickLength : MinorTickLength;
        //    double angle = Map(v, Min, Max, StartAngle, EndAngle);

        //    canvas.SaveState();
        //    canvas.Translate(cx, cy);
        //    canvas.Rotate((float)angle);

        //    // draw tick (white)
        //    canvas.StrokeColor = Colors.White;
        //    canvas.StrokeSize = isMajor ? 2 : 1;
        //    canvas.DrawLine(
        //        0f, -radius,
        //        0f, -radius + len
        //    );

        //    // draw label for major ticks
        //    if (isMajor)
        //    {
        //        var text = ((int)v).ToString();
        //        canvas.FontSize = 16;
        //        canvas.FontColor = v >= Threshold ? Colors.Red : Colors.White;
        //        canvas.DrawString(
        //            text,
        //            0, -radius + len - LabelOffset,
        //            HorizontalAlignment.Center);
        //    }

        //    canvas.RestoreState();
        //}

        //// draw needle (OrangeRed)
        //double clamped = Math.Clamp(Value, Min, Max);
        //double needleAngle = Map(clamped, Min, Max, StartAngle, EndAngle);
        //canvas.SaveState();
        //canvas.Translate(cx, cy);
        //canvas.Rotate((float)needleAngle);

        //canvas.StrokeColor = Colors.OrangeRed;
        //canvas.StrokeSize = 4;
        //canvas.DrawLine(
        //    0f, 0f,
        //    0f, -radius + MajorTickLength
        //);

        //canvas.RestoreState();
        //canvas.RestoreState();
    }

    double Map(double v, double min, double max, double a, double b)
        => a + (v - min) / (max - min) * (b - a);
}
