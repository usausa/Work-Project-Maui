namespace WorkDesign;

using System.Diagnostics;

public partial class JoystickControl : ContentView
{
    // TODO Gradation?
    public static readonly BindableProperty ThumbColorProperty = BindableProperty.Create(
        nameof(ThumbColor),
        typeof(Color),
        typeof(JoystickControl),
        Colors.Red);

    public Color ThumbColor
    {
        get => (Color)GetValue(ThumbColorProperty);
        set => SetValue(ThumbColorProperty, value);
    }

    public static readonly BindableProperty BaseColorProperty = BindableProperty.Create(
        nameof(BaseColor),
        typeof(Color),
        typeof(JoystickControl),
        Colors.Gray);

    public Color BaseColor
    {
        get => (Color)GetValue(BaseColorProperty);
        set => SetValue(BaseColorProperty, value);
    }

    private double radius = 80;

    public JoystickControl()
	{
		InitializeComponent();

        var panGesture = new PanGestureRecognizer();
        panGesture.PanUpdated += OnPanUpdated;
        this.GestureRecognizers.Add(panGesture);
    }

    private void OnPanUpdated(object? sender, PanUpdatedEventArgs e)
    {
        switch (e.StatusType)
        {
            case GestureStatus.Running:
                var (x, y) = ClampToCircle(e.TotalX, e.TotalY);
                Thumb.TranslationX = x;
                Thumb.TranslationY = y;
                break;

            case GestureStatus.Completed:
                var direction = DirectionHelper.GetDirection(Thumb.TranslationX, Thumb.TranslationY);
                Debug.WriteLine(direction);
                Thumb.TranslationX = 0;
                Thumb.TranslationY = 0;
                break;
        }
    }

    public (double X, double Y) ClampToCircle(double x, double y)
    {
        // 現在の座標と中心(0,0)の距離を計算
        double distance = Math.Sqrt(x * x + y * y);

        // 距離が半径より大きい場合にのみ補正を行う
        if (distance > radius)
        {
            // 正規化されたベクトルに半径を乗算して円周上の座標を求める
            double newX = x * (radius / distance);
            double newY = y * (radius / distance);
            return (newX, newY);
        }
        else
        {
            // 範囲内の場合はそのままの座標を返す
            return (x, y);
        }
    }
}

public enum StickDirection
{
    None,
    Up,
    Down,
    Left,
    Right
}

public static class DirectionHelper
{
    private static double sensitivity = 20;

    public static StickDirection GetDirection(double x, double y)
    {
        if (Math.Abs(x) > Math.Abs(y))
            return x > sensitivity ? StickDirection.Right : x < -sensitivity ? StickDirection.Left : StickDirection.None;
        else
            return y > sensitivity ? StickDirection.Down : y < -sensitivity ? StickDirection.Up : StickDirection.None;
    }
}