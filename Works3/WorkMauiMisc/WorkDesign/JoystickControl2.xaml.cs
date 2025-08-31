using System.Diagnostics;

namespace WorkDesign;

using Microsoft.Maui.Controls.Shapes;

public partial class JoystickControl2 : ContentView
{
    private double _thumbRadius;

    // 色のプロパティ
    public static readonly BindableProperty BaseBackgroundColorProperty =
        BindableProperty.Create(nameof(BaseBackgroundColor), typeof(Color), typeof(JoystickControl2), Color.FromArgb("#E0E0E0"));
    public static readonly BindableProperty BaseFillColorProperty =
        BindableProperty.Create(nameof(BaseFillColor), typeof(Color), typeof(JoystickControl2), Colors.White);
    public static readonly BindableProperty BaseStrokeColorProperty =
        BindableProperty.Create(nameof(BaseStrokeColor), typeof(Color), typeof(JoystickControl2), Color.FromArgb("#9E9E9E"));
    public static readonly BindableProperty TriangleColorProperty =
        BindableProperty.Create(nameof(TriangleColor), typeof(Color), typeof(JoystickControl2), Colors.White);
    public static readonly BindableProperty ThumbColorProperty =
        BindableProperty.Create(nameof(ThumbColor), typeof(Color), typeof(JoystickControl2), Color.FromArgb("#607D8B"));

    // 座標のプロパティ
    public static readonly BindableProperty XValueProperty =
        BindableProperty.Create(nameof(XValue), typeof(double), typeof(JoystickControl2), 0.0, defaultBindingMode: BindingMode.OneWayToSource);
    public static readonly BindableProperty YValueProperty =
        BindableProperty.Create(nameof(YValue), typeof(double), typeof(JoystickControl2), 0.0, defaultBindingMode: BindingMode.OneWayToSource);

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

    public JoystickControl2()
    {
        InitializeComponent();
        this.SizeChanged += OnJoystickControl2SizeChanged;
        var panGesture = new PanGestureRecognizer();
        panGesture.PanUpdated += OnPanUpdated;
        Thumb.GestureRecognizers.Add(panGesture);
    }

    private void OnJoystickControl2SizeChanged(object? sender, EventArgs e)
    {
        double containerSize = Math.Min(this.Width, this.Height);
        if (containerSize <= 0) return;

        double thumbSize = containerSize * 0.3;
        //Thumb.WidthRequest = thumbSize;
        //Thumb.HeightRequest = thumbSize;
        _thumbRadius = thumbSize / 2;

        double triangleWidth = containerSize * 0.15;
        double triangleHeight = containerSize * 0.1;

        var converter = new PathGeometryConverter();
        // PathGeometryの定義（前回の回答と同じロジック）
        TopTriangle.Data = (Geometry)converter.ConvertFromInvariantString($"M0,{triangleHeight} L{triangleWidth}, {triangleHeight} L{triangleWidth / 2},0 Z")!;
        BottomTriangle.Data = (Geometry)converter.ConvertFromInvariantString($"M0,0 L{triangleWidth},0 L{triangleWidth / 2},{triangleHeight} Z")!;
        LeftTriangle.Data = (Geometry)converter.ConvertFromInvariantString($"M{triangleHeight},0 L{triangleHeight},{triangleWidth} L0,{triangleWidth / 2} Z")!;
        RightTriangle.Data = (Geometry)converter.ConvertFromInvariantString($"M0,0 L0,{triangleWidth} L{triangleHeight},{triangleWidth / 2} Z")!;

        // 三角形を外周に移動させるためのTranslationを設定
        double outerOffset = (containerSize / 2) - (triangleHeight * 2.5);
        TopTriangle.TranslationY = -outerOffset;
        BottomTriangle.TranslationY = outerOffset;
        LeftTriangle.TranslationX = -outerOffset;
        RightTriangle.TranslationX = outerOffset;
    }

    private void OnPanUpdated(object? sender, PanUpdatedEventArgs e)
    {
        double thumbRadius = Thumb.Width / 2;
        double containerRadius = Math.Min(this.Width, this.Height) / 2;

        switch (e.StatusType)
        {
            case GestureStatus.Running:
                double x = e.TotalX;
                double y = e.TotalY;

                double distance = Math.Sqrt(x * x + y * y);
                if (distance > (containerRadius - thumbRadius))
                {
                    double angle = Math.Atan2(y, x);
                    x = (containerRadius - thumbRadius) * Math.Cos(angle);
                    y = (containerRadius - thumbRadius) * Math.Sin(angle);
                }

                Thumb.TranslationX = x;
                Thumb.TranslationY = y;

                // 座標プロパティを正規化して更新
                XValue = x / (containerRadius - thumbRadius);
                YValue = y / (containerRadius - thumbRadius);

                break;

            case GestureStatus.Completed:
                Thumb.TranslationX = 0;
                Thumb.TranslationY = 0;

                // ドラグ終了時に値をリセット
                XValue = 0;
                YValue = 0;
                break;
        }
    }
}