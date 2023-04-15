namespace ControlExample;

public class Graphic2Drawable : IDrawable
{
    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        // Solid
        var solidPaint = new SolidPaint(Colors.Silver);
        var solidRectangle = new RectF(10, 10, 200, 100);
        canvas.SetFillPaint(solidPaint, solidRectangle);
        canvas.SetShadow(new SizeF(10, 10), 10, Colors.Grey);
        canvas.FillRoundedRectangle(solidRectangle, 12);

        // Pattern 10x10 template
        using var picture = new PictureCanvas(0, 0, 10, 10);
        picture.StrokeColor = Colors.Silver;
        picture.DrawLine(0, 0, 10, 10);
        picture.DrawLine(0, 10, 10, 0);

        var patternPaint = new PatternPaint
        {
            Pattern = new PicturePattern(picture.Picture, 10, 10)
        };
        canvas.SetFillPaint(patternPaint, RectF.Zero);
        canvas.FillRectangle(10, 110, 200, 100);

        // Gradient
        var linearGradientPaint1 = new LinearGradientPaint
        {
            StartColor = Colors.Yellow,
            EndColor = Colors.Green,
            StartPoint = new Point(0, 0),
            EndPoint = new Point(1, 1)
        };
        linearGradientPaint1.AddOffset(0.25f, Colors.Red);
        linearGradientPaint1.AddOffset(0.75f, Colors.Blue);

        var linearRectangle1 = new RectF(10, 210, 200, 100);
        canvas.SetFillPaint(linearGradientPaint1, linearRectangle1);
        canvas.SetShadow(new SizeF(10, 10), 10, Colors.Grey);
        canvas.FillRoundedRectangle(linearRectangle1, 12);

        // Gradient
        var linearGradientPaint2 = new LinearGradientPaint
        {
            StartColor = Colors.Yellow,
            EndColor = Colors.Green,
            EndPoint = new Point(0, 1)
        };

        var linearRectangle2 = new RectF(10, 310, 200, 100);
        canvas.SetFillPaint(linearGradientPaint2, linearRectangle2);
        canvas.SetShadow(new SizeF(10, 10), 10, Colors.Grey);
        canvas.FillRoundedRectangle(linearRectangle2, 12);

        // Gradient
        var linearGradientPaint3 = new LinearGradientPaint
        {
            StartColor = Colors.Yellow,
            EndColor = Colors.Green,
            StartPoint = new Point(0, 0),
            EndPoint = new Point(1, 1)
        };

        var linearRectangle3 = new RectF(10, 410, 200, 100);
        canvas.SetFillPaint(linearGradientPaint3, linearRectangle3);
        canvas.SetShadow(new SizeF(10, 10), 10, Colors.Grey);
        canvas.FillRoundedRectangle(linearRectangle3, 12);

        // Gradient
        var radialGradientPaint = new RadialGradientPaint
        {
            StartColor = Colors.Red,
            EndColor = Colors.DarkBlue,
            Center = new Point(0.5, 0.5),
            Radius = 0.5
        };

        var radialRectangle = new RectF(10, 510, 200, 100);
        canvas.SetFillPaint(radialGradientPaint, radialRectangle);
        canvas.SetShadow(new SizeF(10, 10), 10, Colors.Grey);
        canvas.FillRoundedRectangle(radialRectangle, 12);
    }
}
