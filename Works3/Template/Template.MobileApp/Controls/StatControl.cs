namespace Template.MobileApp.Controls;

public sealed class StatControl : GraphicsView, IDrawable
{
    private const string FlowAnimationName = "StatControlFlow";

    private const int GridLines = 3;

    // 前の値が届いてからの時間 (ms) の範囲。この時間で 1 区間ぶん流れる
    private const long MinFlowLength = 100;

    private const long MaxFlowLength = 3000;

    private const long DefaultFlowLength = 1000;

    private float phase = 1f;

    private long updatedAt;

    // ------------------------------------------------------------
    // Property
    // ------------------------------------------------------------

    public static readonly BindableProperty GraphColorProperty = BindableProperty.Create(
        nameof(GraphColor),
        typeof(Color),
        typeof(StatControl),
        Colors.Blue,
        propertyChanged: OnPropertyChanged);

    public Color GraphColor
    {
        get => (Color)GetValue(GraphColorProperty);
        set => SetValue(GraphColorProperty, value);
    }

    public static readonly BindableProperty HeaderHeightProperty = BindableProperty.Create(
        nameof(HeaderHeight),
        typeof(int),
        typeof(StatControl),
        32,
        propertyChanged: OnPropertyChanged);

    public int HeaderHeight
    {
        get => (int)GetValue(HeaderHeightProperty);
        set => SetValue(HeaderHeightProperty, value);
    }

    public static readonly BindableProperty HeaderPaddingProperty = BindableProperty.Create(
        nameof(HeaderPadding),
        typeof(float),
        typeof(StatControl),
        12f,
        propertyChanged: OnPropertyChanged);

    public float HeaderPadding
    {
        get => (float)GetValue(HeaderPaddingProperty);
        set => SetValue(HeaderPaddingProperty, value);
    }

    public static readonly BindableProperty LabelFontSizeProperty = BindableProperty.Create(
        nameof(LabelFontSize),
        typeof(float),
        typeof(StatControl),
        14f,
        propertyChanged: OnPropertyChanged);

    public float LabelFontSize
    {
        get => (float)GetValue(LabelFontSizeProperty);
        set => SetValue(LabelFontSizeProperty, value);
    }

    public static readonly BindableProperty ValueFontSizeProperty = BindableProperty.Create(
        nameof(ValueFontSize),
        typeof(float),
        typeof(StatControl),
        24f,
        propertyChanged: OnPropertyChanged);

    public float ValueFontSize
    {
        get => (float)GetValue(ValueFontSizeProperty);
        set => SetValue(ValueFontSizeProperty, value);
    }

    public static readonly BindableProperty LabelProperty = BindableProperty.Create(
        nameof(Label),
        typeof(string),
        typeof(StatControl),
        "Usage",
        propertyChanged: OnPropertyChanged);

    public string Label
    {
        get => (string)GetValue(LabelProperty);
        set => SetValue(LabelProperty, value);
    }

    public static readonly BindableProperty UnitProperty = BindableProperty.Create(
        nameof(Unit),
        typeof(string),
        typeof(StatControl),
        "%",
        propertyChanged: OnPropertyChanged);

    public string Unit
    {
        get => (string)GetValue(UnitProperty);
        set => SetValue(UnitProperty, value);
    }

    public static readonly BindableProperty ValueFormatProperty = BindableProperty.Create(
        nameof(ValueFormat),
        typeof(string),
        typeof(StatControl),
        "F1",
        propertyChanged: OnPropertyChanged);

    public string ValueFormat
    {
        get => (string)GetValue(ValueFormatProperty);
        set => SetValue(ValueFormatProperty, value);
    }

    public static readonly BindableProperty MaxValueProperty = BindableProperty.Create(
        nameof(MaxValue),
        typeof(float),
        typeof(StatControl),
        100.0f,
        propertyChanged: OnPropertyChanged);

    public float MaxValue
    {
        get => (float)GetValue(MaxValueProperty);
        set => SetValue(MaxValueProperty, value);
    }

    public static readonly BindableProperty DataSetProperty = BindableProperty.Create(
        nameof(DataSet),
        typeof(StatDataSet),
        typeof(StatControl),
        propertyChanged: OnDataSetChanged);

    public StatDataSet DataSet
    {
        get => (StatDataSet)GetValue(DataSetProperty);
        set => SetValue(DataSetProperty, value);
    }

    // ------------------------------------------------------------
    // Constructor
    // ------------------------------------------------------------

    public StatControl()
    {
        Drawable = this;
    }

    // ------------------------------------------------------------
    // Handler
    // ------------------------------------------------------------

    private static void OnPropertyChanged(BindableObject bindable, object oldValue, object newValue)
    {
        ((StatControl)bindable).Invalidate();
    }

    private static void OnDataSetChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (StatControl)bindable;

        if (oldValue is StatDataSet oldDataSet)
        {
            oldDataSet.Updated -= control.OnDataSetUpdated;
        }

        if (newValue is StatDataSet newDataSet)
        {
            newDataSet.Updated += control.OnDataSetUpdated;
        }
    }

    private void OnDataSetUpdated(object? sender, EventArgs e)
    {
        var now = Environment.TickCount64;
        var length = updatedAt == 0 ? DefaultFlowLength : Math.Clamp(now - updatedAt, MinFlowLength, MaxFlowLength);
        updatedAt = now;

        this.AbortAnimation(FlowAnimationName);
        phase = 0f;
        this.Animate(
            FlowAnimationName,
            v =>
            {
                phase = (float)v;
                Invalidate();
            },
            0d,
            1d,
            16,
            (uint)length,
            Easing.Linear);
    }

    // ------------------------------------------------------------
    // Draw
    // ------------------------------------------------------------

    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        var width = dirtyRect.Width;
        var height = dirtyRect.Height;
        var statHeight = height - HeaderHeight;
        var headerPadding = HeaderPadding;
        var headerRect = new RectF(headerPadding, headerPadding, width - (headerPadding * 2), HeaderHeight);

        var values = DataSet;
        var pointWidth = width / (values.Capacity - 1);
        var shift = (1f - phase) * pointWidth;
        var maxValueForScale = MaxValue > 0 ? MaxValue : 100f;
        var color = GraphColor;

        canvas.SaveState();
        canvas.Antialias = true;

        // Background
        var backgroundPaint = new LinearGradientPaint
        {
            StartPoint = new Point(0, 0),
            EndPoint = new Point(1, 0),
            GradientStops = [new PaintGradientStop(0.0f, color.WithAlpha(1.0f)), new PaintGradientStop(1.0f, color.WithAlpha(192f / 255f))]
        };
        canvas.SetFillPaint(backgroundPaint, dirtyRect);
        canvas.FillRectangle(dirtyRect);

        // 目盛り(データが無くても出す)
        canvas.StrokeColor = Colors.White.WithAlpha(0.25f);
        canvas.StrokeSize = 1;
        for (var i = 1; i <= GridLines; i++)
        {
            var y = height - (statHeight * i / (GridLines + 1));
            canvas.DrawLine(0, y, width, y);
        }

        // Path (最新の値は右の外から入り、古い側は左の端まで平らにつなぐ)
        var wavePath = new PathF();
        wavePath.MoveTo(0, height);
        wavePath.LineTo(0, height - (values.GetValue(0) / maxValueForScale * statHeight));

        for (var i = 0; i < values.Capacity; i++)
        {
            var x = (i * pointWidth) + shift;
            var normalizedValue = values.GetValue(i) / maxValueForScale;
            var y = height - (normalizedValue * statHeight);
            wavePath.LineTo(x, y);
        }

        wavePath.LineTo(width + shift, height);
        wavePath.Close();

        // Gradation
        var waveGradient = new LinearGradientPaint
        {
            StartPoint = new Point(0, 0),
            EndPoint = new Point(0, 1),
            GradientStops = [new PaintGradientStop(0.0f, Colors.White.WithAlpha(192f / 255f)), new PaintGradientStop(1.0f, Colors.White.WithAlpha(64f / 255f))]
        };
        canvas.SetFillPaint(waveGradient, dirtyRect);
        canvas.FillPath(wavePath);

        // Line
        using var linePath = new PathF();
        linePath.MoveTo(0, height - (values.GetValue(0) / maxValueForScale * statHeight));

        for (var i = 0; i < values.Capacity; i++)
        {
            var x = (i * pointWidth) + shift;
            var y = height - (values.GetValue(i) / maxValueForScale * statHeight);
            linePath.LineTo(x, y);
        }

        canvas.StrokeColor = Colors.White;
        canvas.StrokeSize = 1.5f;
        canvas.DrawPath(linePath);

        // Label
        canvas.Font = Microsoft.Maui.Graphics.Font.DefaultBold;
        canvas.FontColor = Colors.White;
        canvas.FontSize = LabelFontSize;
        canvas.DrawString(Label, headerRect, HorizontalAlignment.Left, VerticalAlignment.Top);
        canvas.Font = Microsoft.Maui.Graphics.Font.Default;

        // Value
        var currentValue = values.LastValue;
        var unit = Unit;
        var value = currentValue.ToString(ValueFormat, CultureInfo.CurrentCulture);
        var valueText = String.IsNullOrEmpty(unit) ? value : $"{value} {unit}";

        canvas.FontColor = Colors.White;
        canvas.FontSize = ValueFontSize;
        canvas.DrawString(valueText, headerRect, HorizontalAlignment.Right, VerticalAlignment.Top);

        canvas.RestoreState();
    }
}
