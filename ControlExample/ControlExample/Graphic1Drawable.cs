namespace ControlExample;

using Font = Microsoft.Maui.Graphics.Font;

internal class Graphic1Drawable : IDrawable
{
    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        canvas.StrokeColor = Colors.Red;
        canvas.StrokeSize = 4;
        canvas.StrokeDashPattern = new float[] { 2, 2 };
        canvas.DrawLine(10, 10, 100, 50);
        canvas.StrokeDashPattern = null;

        canvas.StrokeColor = Colors.Red;
        canvas.StrokeSize = 4;
        canvas.DrawEllipse(10, 60, 50, 50);

        canvas.FillColor = Colors.Red;
        canvas.FillEllipse(210, 60, 150, 50);

        canvas.StrokeColor = Colors.DarkBlue;
        canvas.StrokeSize = 4;
        canvas.DrawRectangle(10, 110, 100, 50);

        canvas.FillColor = Colors.DarkBlue;
        canvas.FillRectangle(210, 110, 100, 50);

        canvas.StrokeColor = Colors.Green;
        canvas.StrokeSize = 4;
        canvas.DrawRoundedRectangle(10, 160, 100, 50, 12);

        canvas.FillColor = Colors.Green;
        canvas.FillRoundedRectangle(210, 160, 100, 50, 12);

        canvas.StrokeColor = Colors.Teal;
        canvas.StrokeSize = 4;
        canvas.DrawArc(10, 160, 100, 100, 0, 180, true, false);

        canvas.FillColor = Colors.Teal;
        canvas.FillArc(210, 160, 100, 100, 0, 180, true);

        using var path1 = new PathF();
        path1.MoveTo(40, 260);
        path1.LineTo(70, 330);
        path1.LineTo(10, 300);
        path1.Close();
        canvas.StrokeColor = Colors.Green;
        canvas.StrokeSize = 6;
        canvas.DrawPath(path1);

        using var path2 = new PathF();
        path2.MoveTo(240, 260);
        path2.LineTo(270, 330);
        path2.LineTo(210, 300);
        canvas.FillColor = Colors.SlateBlue;
        canvas.FillPath(path2);

        canvas.FontColor = Colors.Blue;
        canvas.FontSize = 18;

        canvas.Font = Font.Default;
        canvas.DrawString("Text is left aligned.", 20, 360, 380, 100, HorizontalAlignment.Left, VerticalAlignment.Top);
        canvas.DrawString("Text is centered.", 20, 390, 380, 100, HorizontalAlignment.Center, VerticalAlignment.Top);
        canvas.DrawString("Text is right aligned.", 20, 420, 380, 100, HorizontalAlignment.Right, VerticalAlignment.Top);

        canvas.Font = Font.DefaultBold;
        canvas.DrawString("This text is displayed using the bold system font.", 20, 460, 350, 100, HorizontalAlignment.Left, VerticalAlignment.Top);

        canvas.Font = new Font("Arial");
        canvas.FontColor = Colors.Black;
        canvas.SetShadow(new SizeF(6, 6), 4, Colors.Gray);
        canvas.DrawString("This text has a shadow.", 20, 520, 300, 100, HorizontalAlignment.Left, VerticalAlignment.Top);

        canvas.Font = new Font("Arial");
        canvas.FontSize = 18;
        canvas.FontColor = Colors.Blue;

        canvas.FillColor = Colors.Red;
        canvas.SetShadow(new SizeF(10, 10), 4, Colors.Grey);
        canvas.FillRectangle(10, 560, 90, 100);

        canvas.FillColor = Colors.Green;
        canvas.SetShadow(new SizeF(10, -10), 4, Colors.Grey);
        canvas.FillEllipse(110, 560, 90, 100);

        canvas.FillColor = Colors.Blue;
        canvas.SetShadow(new SizeF(-10, 10), 4, Colors.Grey);
        canvas.FillRoundedRectangle(210, 560, 90, 100, 25);
    }
}
