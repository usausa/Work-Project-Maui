namespace WorkGraphicsShape;

public partial class MainPage : ContentPage, IDrawable
{
    public MainPage()
    {
        InitializeComponent();

        GraphicsView.Drawable = this;
    }

    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        canvas.Scale(2, 2);

        canvas.FillColor = Colors.SpringGreen;
        canvas.FillRectangle(0, 0, 100, 100);
        // TODO
    }
}
