namespace WorkVisualMusic;

using Microsoft.Maui.Graphics;

using System.Windows.Input;

public class VerticalSlider : GraphicsView
{
    // バインド可能なプロパティ
    public static readonly BindableProperty MinimumProperty =
        BindableProperty.Create(nameof(Minimum), typeof(double), typeof(VerticalSlider), 0.0,
            propertyChanged: OnSliderPropertyChanged);

    public static readonly BindableProperty MaximumProperty =
        BindableProperty.Create(nameof(Maximum), typeof(double), typeof(VerticalSlider), 100.0,
            propertyChanged: OnSliderPropertyChanged);

    public static readonly BindableProperty ValueProperty =
        BindableProperty.Create(nameof(Value), typeof(double), typeof(VerticalSlider), 0.0,
            propertyChanged: OnValueChanged, defaultBindingMode: BindingMode.TwoWay,
            coerceValue: CoerceValue);

    public static readonly BindableProperty TrackColorProperty =
        BindableProperty.Create(nameof(TrackColor), typeof(Color), typeof(VerticalSlider), Colors.LightGray,
            propertyChanged: OnSliderPropertyChanged);

    public static readonly BindableProperty ProgressColorProperty =
        BindableProperty.Create(nameof(ProgressColor), typeof(Color), typeof(VerticalSlider), Colors.DodgerBlue,
            propertyChanged: OnSliderPropertyChanged);

    public static readonly BindableProperty ThumbColorProperty =
        BindableProperty.Create(nameof(ThumbColor), typeof(Color), typeof(VerticalSlider), Colors.White,
            propertyChanged: OnSliderPropertyChanged);

    public static readonly BindableProperty ThumbBorderColorProperty =
        BindableProperty.Create(nameof(ThumbBorderColor), typeof(Color), typeof(VerticalSlider), Colors.DarkGray,
            propertyChanged: OnSliderPropertyChanged);

    public static readonly BindableProperty ThumbRadiusProperty =
        BindableProperty.Create(nameof(ThumbRadius), typeof(float), typeof(VerticalSlider), 12.0f,
            propertyChanged: OnSliderPropertyChanged);

    public static readonly BindableProperty TrackWidthProperty =
        BindableProperty.Create(nameof(TrackWidth), typeof(float), typeof(VerticalSlider), 6.0f,
            propertyChanged: OnSliderPropertyChanged);

    public static readonly BindableProperty ValueChangedCommandProperty =
        BindableProperty.Create(nameof(ValueChangedCommand), typeof(ICommand), typeof(VerticalSlider), null);

    // プロパティアクセサ
    public double Minimum
    {
        get => (double)GetValue(MinimumProperty);
        set => SetValue(MinimumProperty, value);
    }

    public double Maximum
    {
        get => (double)GetValue(MaximumProperty);
        set => SetValue(MaximumProperty, value);
    }

    public double Value
    {
        get => (double)GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    public Color TrackColor
    {
        get => (Color)GetValue(TrackColorProperty);
        set => SetValue(TrackColorProperty, value);
    }

    public Color ProgressColor
    {
        get => (Color)GetValue(ProgressColorProperty);
        set => SetValue(ProgressColorProperty, value);
    }

    public Color ThumbColor
    {
        get => (Color)GetValue(ThumbColorProperty);
        set => SetValue(ThumbColorProperty, value);
    }

    public Color ThumbBorderColor
    {
        get => (Color)GetValue(ThumbBorderColorProperty);
        set => SetValue(ThumbBorderColorProperty, value);
    }

    public float ThumbRadius
    {
        get => (float)GetValue(ThumbRadiusProperty);
        set => SetValue(ThumbRadiusProperty, value);
    }

    public float TrackWidth
    {
        get => (float)GetValue(TrackWidthProperty);
        set => SetValue(TrackWidthProperty, value);
    }

    public ICommand ValueChangedCommand
    {
        get => (ICommand)GetValue(ValueChangedCommandProperty);
        set => SetValue(ValueChangedCommandProperty, value);
    }

    // イベント
    public event EventHandler<ValueChangedEventArgs>? ValueChanged;

    // コンストラクタ
    public VerticalSlider()
    {
        // 描画用のDrawableを設定
        Drawable = new VerticalSliderDrawable(this);

        // タッチイベントを処理
        StartInteraction += OnStartInteraction;
        DragInteraction += OnDragInteraction;
        EndInteraction += OnEndInteraction;
    }

    // スライダーの値を変更する内部メソッド
    private void UpdateValue(PointF point)
    {
        // タッチ位置からスライダーの値を計算
        // 縦スライダーなので、上が最大値、下が最小値となる
        var trackHeight = Height - (ThumbRadius * 2);
        var thumbPosition = point.Y - ThumbRadius;

        // 位置から値を計算（逆転させる：上が最大値、下が最小値）
        var percentage = 1 - Math.Clamp(thumbPosition / trackHeight, 0, 1);
        var newValue = Minimum + (percentage * (Maximum - Minimum));

        Value = Math.Round(newValue, 2); // 小数点2桁に丸める
    }

    // タッチイベントハンドラー
    private void OnStartInteraction(object? sender, TouchEventArgs e)
    {
        UpdateValue(e.Touches.First());
    }

    private void OnDragInteraction(object? sender, TouchEventArgs e)
    {
        UpdateValue(e.Touches.First());
    }

    private void OnEndInteraction(object? sender, TouchEventArgs e)
    {
        // ドラッグ終了時も値を更新
        UpdateValue(e.Touches.First());
    }

    // プロパティ変更時の処理
    private static void OnSliderPropertyChanged(BindableObject bindable, object oldValue, object newValue)
    {
        // スライダーの再描画を行う
        ((VerticalSlider)bindable).Invalidate();
    }

    private static void OnValueChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var slider = (VerticalSlider)bindable;
        var oldDoubleValue = (double)oldValue;
        var newDoubleValue = (double)newValue;

        // ValueChangedイベントを発火
        slider.ValueChanged?.Invoke(slider, new ValueChangedEventArgs(oldDoubleValue, newDoubleValue));

        // コマンドがあれば実行
        if (slider.ValueChangedCommand?.CanExecute(newDoubleValue) == true)
        {
            slider.ValueChangedCommand.Execute(newDoubleValue);
        }

        // スライダーの再描画を行う
        slider.Invalidate();
    }

    // 値の範囲制限
    private static object CoerceValue(BindableObject bindable, object value)
    {
        var slider = (VerticalSlider)bindable;
        var doubleValue = (double)value;

        if (doubleValue < slider.Minimum)
            return slider.Minimum;

        if (doubleValue > slider.Maximum)
            return slider.Maximum;

        return doubleValue;
    }
}

// 値変更時のイベント引数
public class ValueChangedEventArgs : EventArgs
{
    public double OldValue { get; }
    public double NewValue { get; }

    public ValueChangedEventArgs(double oldValue, double newValue)
    {
        OldValue = oldValue;
        NewValue = newValue;
    }
}

// スライダーの描画を担当するクラス
internal class VerticalSliderDrawable : IDrawable
{
    private readonly VerticalSlider _slider;

    public VerticalSliderDrawable(VerticalSlider slider)
    {
        _slider = slider;
    }

    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        // トラックの中心X座標
        var centerX = dirtyRect.Width / 2;

        // スライダーの値に基づくサムの位置を計算
        var valuePercentage = (_slider.Value - _slider.Minimum) / (_slider.Maximum - _slider.Minimum);

        // トラックのY座標範囲（サムの半径分のマージンを取る）
        var trackTop = _slider.ThumbRadius;
        var trackBottom = dirtyRect.Height - _slider.ThumbRadius;
        var trackHeight = trackBottom - trackTop;

        // サムのY座標（上が最大値、下が最小値）
        var thumbY = trackBottom - (float)(valuePercentage * trackHeight);

        // トラックを描画（背景部分）
        canvas.StrokeSize = _slider.TrackWidth;
        canvas.StrokeColor = _slider.TrackColor;
        canvas.DrawLine(centerX, trackTop, centerX, trackBottom);

        // 進捗部分のトラックを描画（選択値までの部分）
        canvas.StrokeColor = _slider.ProgressColor;
        canvas.DrawLine(centerX, thumbY, centerX, trackBottom);

        // サムを描画
        canvas.FillColor = _slider.ThumbColor;
        canvas.StrokeColor = _slider.ThumbBorderColor;
        canvas.StrokeSize = 1;
        canvas.FillCircle(centerX, thumbY, _slider.ThumbRadius);
        canvas.DrawCircle(centerX, thumbY, _slider.ThumbRadius);
    }
}