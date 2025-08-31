namespace WorkDesign;

using System.Diagnostics;

public partial class JoystickControl : ContentView
{
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

    // 色のプロパティ
    public static readonly BindableProperty BaseBackgroundColorProperty =
        BindableProperty.Create(nameof(BaseBackgroundColor), typeof(Color), typeof(JoystickControl), Color.FromArgb("#E0E0E0"));
    public static readonly BindableProperty BaseFillColorProperty =
        BindableProperty.Create(nameof(BaseFillColor), typeof(Color), typeof(JoystickControl), Colors.White);
    public static readonly BindableProperty BaseStrokeColorProperty =
        BindableProperty.Create(nameof(BaseStrokeColor), typeof(Color), typeof(JoystickControl), Color.FromArgb("#9E9E9E"));
    public static readonly BindableProperty TriangleColorProperty =
        BindableProperty.Create(nameof(TriangleColor), typeof(Color), typeof(JoystickControl), Colors.White);
    public static readonly BindableProperty ThumbColorProperty =
        BindableProperty.Create(nameof(ThumbColor), typeof(Color), typeof(JoystickControl), Color.FromArgb("#607D8B"));

    // 座標のプロパティ
    public static readonly BindableProperty XValueProperty =
        BindableProperty.Create(nameof(XValue), typeof(double), typeof(JoystickControl), 0.0, defaultBindingMode: BindingMode.OneWayToSource);
    public static readonly BindableProperty YValueProperty =
        BindableProperty.Create(nameof(YValue), typeof(double), typeof(JoystickControl), 0.0, defaultBindingMode: BindingMode.OneWayToSource);

    // プロパティ公開
    public Color BaseBackgroundColor
    {
        get => (Color)GetValue(BaseBackgroundColorProperty);
        set => SetValue(BaseBackgroundColorProperty, value);
    }
    public Color BaseFillColor
    {
        get => (Color)GetValue(BaseFillColorProperty);
        set => SetValue(BaseFillColorProperty, value);
    }
    public Color BaseStrokeColor
    {
        get => (Color)GetValue(BaseStrokeColorProperty);
        set => SetValue(BaseStrokeColorProperty, value);
    }
    public Color TriangleColor
    {
        get => (Color)GetValue(TriangleColorProperty);
        set => SetValue(TriangleColorProperty, value);
    }
    public Color ThumbColor
    {
        get => (Color)GetValue(ThumbColorProperty);
        set => SetValue(ThumbColorProperty, value);
    }
    public double XValue
    {
        get => (double)GetValue(XValueProperty);
        private set => SetValue(XValueProperty, value);
    }
    public double YValue
    {
        get => (double)GetValue(YValueProperty);
        private set => SetValue(YValueProperty, value);
    }

    public static readonly BindableProperty ThumbFillProperty =
        BindableProperty.Create(nameof(ThumbFill), typeof(Brush), typeof(JoystickControl), new SolidColorBrush(Color.FromArgb("#607D8B")));

    public Brush ThumbFill
    {
        get => (Brush)GetValue(ThumbFillProperty);
        set => SetValue(ThumbFillProperty, value);
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