namespace WorkDesign;

using Smart.Maui.Interactivity;

// TODO 方向制限、左右、上下、矢印表示も

public partial class DPadCircleView : ContentView
{
    public static readonly BindableProperty XValueProperty =
        BindableProperty.Create(nameof(XValue), typeof(double), typeof(DPadCircleView), 0.0,
            defaultBindingMode: BindingMode.OneWayToSource);

    public static readonly BindableProperty YValueProperty =
        BindableProperty.Create(nameof(YValue), typeof(double), typeof(DPadCircleView), 0.0,
            defaultBindingMode: BindingMode.OneWayToSource);

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

    public static readonly BindableProperty ThumbColorProperty =
        BindableProperty.Create(nameof(ThumbColor), typeof(Color), typeof(DPadCircleView), Color.FromArgb("#607D8B"));

    public Color ThumbColor
    {
        get => (Color)GetValue(ThumbColorProperty);
        set => SetValue(ThumbColorProperty, value);
    }

    public static readonly BindableProperty BaseBackgroundProperty =
        BindableProperty.Create(nameof(BaseBackground), typeof(Brush), typeof(DPadCircleView),
            propertyChanged: OnVisualPropertyChanged);

    // 三角形の色
    public static readonly BindableProperty TriangleColorProperty =
        BindableProperty.Create(nameof(TriangleColor), typeof(Color), typeof(DPadCircleView),
            Color.FromArgb("#444444"));

    // 係数（例のルール通り）
    // - MarginRatio: 円周から内側へのオフセット（頂点）
    // - TriangleHeightRatio: 頂点から底辺までの高さ
    // - TriangleHalfWidthRatio: 底辺の半幅
    public static readonly BindableProperty MarginRatioProperty =
        BindableProperty.Create(nameof(MarginRatio), typeof(double), typeof(DPadCircleView),
            0.1d, propertyChanged: OnGeometryPropertyChanged);

    public static readonly BindableProperty TriangleHeightRatioProperty =
        BindableProperty.Create(nameof(TriangleHeightRatio), typeof(double), typeof(DPadCircleView),
            0.2d, propertyChanged: OnGeometryPropertyChanged);

    public static readonly BindableProperty TriangleHalfWidthRatioProperty =
        BindableProperty.Create(nameof(TriangleHalfWidthRatio), typeof(double), typeof(DPadCircleView),
            0.2d, propertyChanged: OnGeometryPropertyChanged);

    public Brush BaseBackground
    {
        get => (Brush)GetValue(BaseBackgroundProperty);
        set => SetValue(BaseBackgroundProperty, value);
    }
    //public Color StartColor
    //{
    //    get => (Color)GetValue(StartColorProperty);
    //    set => SetValue(StartColorProperty, value);
    //}

    //public Color EndColor
    //{
    //    get => (Color)GetValue(EndColorProperty);
    //    set => SetValue(EndColorProperty, value);
    //}

    public Color TriangleColor
    {
        get => (Color)GetValue(TriangleColorProperty);
        set => SetValue(TriangleColorProperty, value);
    }

    public double MarginRatio
    {
        get => (double)GetValue(MarginRatioProperty);
        set => SetValue(MarginRatioProperty, value);
    }

    public double TriangleHeightRatio
    {
        get => (double)GetValue(TriangleHeightRatioProperty);
        set => SetValue(TriangleHeightRatioProperty, value);
    }

    public double TriangleHalfWidthRatio
    {
        get => (double)GetValue(TriangleHalfWidthRatioProperty);
        set => SetValue(TriangleHalfWidthRatioProperty, value);
    }

    private double radius = 56;

    public DPadCircleView()
    {
        InitializeComponent();
        SizeChanged += (_, __) => UpdateGeometry();
        //Loaded += (_, __) => UpdateGeometry();

        SizeChanged += OnJoystickControlSizeChanged;

        var panGesture = new PanGestureRecognizer();
        panGesture.PanUpdated += OnPanUpdated;
        ThumbGrid.GestureRecognizers.Add(panGesture);
    }

    private static void OnVisualPropertyChanged(BindableObject bindable, object oldValue, object newValue)
    {
        // Brush はバインディングしているので、ここではジオメトリのみ更新
        (bindable as DPadCircleView)?.UpdateGeometry();
    }

    private static void OnGeometryPropertyChanged(BindableObject bindable, object oldValue, object newValue)
    {
        (bindable as DPadCircleView)?.UpdateGeometry();
    }

    private void OnJoystickControlSizeChanged(object? sender, EventArgs e)
    {
    }
    private void OnPanUpdated(object? sender, PanUpdatedEventArgs e)
    {
        switch (e.StatusType)
        {
            case GestureStatus.Started:
            case GestureStatus.Running:
                var (x, y) = ClampToCircle(e.TotalX, e.TotalY);
                Thumb.TranslationX = x;
                Thumb.TranslationY = y;

                XValue = x / radius;
                YValue = y / radius;

                break;

            case GestureStatus.Completed:
            case GestureStatus.Canceled:
                Thumb.TranslationX = 0;
                Thumb.TranslationY = 0;

                XValue = 0;
                YValue = 0;

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

    private void UpdateGeometry()
    {
        if (Width <= 0 || Height <= 0)
            return;

        // 中央に最大の正円をレイアウト
        double size = Math.Min(Width, Height);
        double r = size / 2.0;
        double cx = Width / 2.0;
        double cy = Height / 2.0;

        double xLeft = cx - r;
        double xRight = cx + r;
        double yTop = cy - r;
        double yBottom = cy + r;

        // 背景円のサイズを更新（Grid 中央に正円）
        BgCircle.WidthRequest = size;
        BgCircle.HeightRequest = size;

        // 係数から各値を算出（説明のルール）
        double margin = r * MarginRatio;               // 頂点の内側オフセット
        double h = r * TriangleHeightRatio;            // 三角形の高さ
        double w = r * TriangleHalfWidthRatio;         // 底辺の半幅

        // 上
        {
            var apex = new Point(cx, yTop + margin);
            double baseY = apex.Y + h;
            var left = new Point(cx - w, baseY);
            var right = new Point(cx + w, baseY);
            UpTriangle.Points = [apex, left, right];
        }

        // 下
        {
            var apex = new Point(cx, yBottom - margin);
            double baseY = apex.Y - h;
            var left = new Point(cx - w, baseY);
            var right = new Point(cx + w, baseY);
            DownTriangle.Points = [apex, right, left]; // 時計回りでも可
        }

        // 左
        {
            var apex = new Point(xLeft + margin, cy);
            double baseX = apex.X + h;
            var top = new Point(baseX, cy - w);
            var bottom = new Point(baseX, cy + w);
            LeftTriangle.Points = [apex, top, bottom];
        }

        // 右
        {
            var apex = new Point(xRight - margin, cy);
            double baseX = apex.X - h;
            var top = new Point(baseX, cy - w);
            var bottom = new Point(baseX, cy + w);
            RightTriangle.Points = [apex, bottom, top];
        }
    }
}
