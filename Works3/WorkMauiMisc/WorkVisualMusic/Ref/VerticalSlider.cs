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

public class VerticalMixerSlider : GraphicsView
{
    // ドラッグ操作の状態を追跡
    private bool _isDragging = false;

    // つまみの周囲のタッチ判定用のマージン（ピクセル単位）
    private const float THUMB_TOUCH_MARGIN = 10.0f;

    // バインド可能なプロパティ
    public static readonly BindableProperty MinimumProperty =
        BindableProperty.Create(nameof(Minimum), typeof(double), typeof(VerticalMixerSlider), 0.0,
            propertyChanged: OnSliderPropertyChanged);

    public static readonly BindableProperty MaximumProperty =
        BindableProperty.Create(nameof(Maximum), typeof(double), typeof(VerticalMixerSlider), 100.0,
            propertyChanged: OnSliderPropertyChanged);

    public static readonly BindableProperty ValueProperty =
        BindableProperty.Create(nameof(Value), typeof(double), typeof(VerticalMixerSlider), 0.0,
            propertyChanged: OnValueChanged, defaultBindingMode: BindingMode.TwoWay,
            coerceValue: CoerceValue);

    public static readonly BindableProperty TrackColorProperty =
        BindableProperty.Create(nameof(TrackColor), typeof(Color), typeof(VerticalMixerSlider), Colors.DimGray,
            propertyChanged: OnSliderPropertyChanged);

    public static readonly BindableProperty ProgressColorProperty =
        BindableProperty.Create(nameof(ProgressColor), typeof(Color), typeof(VerticalMixerSlider), Colors.OrangeRed,
            propertyChanged: OnSliderPropertyChanged);

    public static readonly BindableProperty ThumbColorProperty =
        BindableProperty.Create(nameof(ThumbColor), typeof(Color), typeof(VerticalMixerSlider), Colors.Silver,
            propertyChanged: OnSliderPropertyChanged);

    public static readonly BindableProperty ThumbBorderColorProperty =
        BindableProperty.Create(nameof(ThumbBorderColor), typeof(Color), typeof(VerticalMixerSlider), Colors.Black,
            propertyChanged: OnSliderPropertyChanged);

    public static readonly BindableProperty ThumbWidthProperty =
        BindableProperty.Create(nameof(ThumbWidth), typeof(float), typeof(VerticalMixerSlider), 30.0f,
            propertyChanged: OnSliderPropertyChanged);

    public static readonly BindableProperty ThumbHeightProperty =
        BindableProperty.Create(nameof(ThumbHeight), typeof(float), typeof(VerticalMixerSlider), 12.0f,
            propertyChanged: OnSliderPropertyChanged);

    public static readonly BindableProperty TrackWidthProperty =
        BindableProperty.Create(nameof(TrackWidth), typeof(float), typeof(VerticalMixerSlider), 6.0f,
            propertyChanged: OnSliderPropertyChanged);

    public static readonly BindableProperty HasTickMarksProperty =
        BindableProperty.Create(nameof(HasTickMarks), typeof(bool), typeof(VerticalMixerSlider), false,
            propertyChanged: OnSliderPropertyChanged);

    public static readonly BindableProperty TickMarkColorProperty =
        BindableProperty.Create(nameof(TickMarkColor), typeof(Color), typeof(VerticalMixerSlider), Colors.Gray,
            propertyChanged: OnSliderPropertyChanged);

    public static readonly BindableProperty TickMarkCountProperty =
        BindableProperty.Create(nameof(TickMarkCount), typeof(int), typeof(VerticalMixerSlider), 11,
            propertyChanged: OnSliderPropertyChanged);

    public static readonly BindableProperty TickMarkLengthProperty =
        BindableProperty.Create(nameof(TickMarkLength), typeof(float), typeof(VerticalMixerSlider), 5.0f,
            propertyChanged: OnSliderPropertyChanged);

    public static readonly BindableProperty ValueChangedCommandProperty =
        BindableProperty.Create(nameof(ValueChangedCommand), typeof(ICommand), typeof(VerticalMixerSlider), null);

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

    public float ThumbWidth
    {
        get => (float)GetValue(ThumbWidthProperty);
        set => SetValue(ThumbWidthProperty, value);
    }

    public float ThumbHeight
    {
        get => (float)GetValue(ThumbHeightProperty);
        set => SetValue(ThumbHeightProperty, value);
    }

    public float TrackWidth
    {
        get => (float)GetValue(TrackWidthProperty);
        set => SetValue(TrackWidthProperty, value);
    }

    public bool HasTickMarks
    {
        get => (bool)GetValue(HasTickMarksProperty);
        set => SetValue(HasTickMarksProperty, value);
    }

    public Color TickMarkColor
    {
        get => (Color)GetValue(TickMarkColorProperty);
        set => SetValue(TickMarkColorProperty, value);
    }

    public int TickMarkCount
    {
        get => (int)GetValue(TickMarkCountProperty);
        set => SetValue(TickMarkCountProperty, value);
    }

    public float TickMarkLength
    {
        get => (float)GetValue(TickMarkLengthProperty);
        set => SetValue(TickMarkLengthProperty, value);
    }

    public ICommand ValueChangedCommand
    {
        get => (ICommand)GetValue(ValueChangedCommandProperty);
        set => SetValue(ValueChangedCommandProperty, value);
    }

    // イベント
    public event EventHandler<ValueChangedEventArgs>? ValueChanged;

    // コンストラクタ
    public VerticalMixerSlider()
    {
        // 描画用のDrawableを設定
        Drawable = new VerticalMixerSliderDrawable(this);

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
        var trackHeight = Height - ThumbHeight;
        var thumbPosition = point.Y - (ThumbHeight / 2);

        // 位置から値を計算（逆転させる：上が最大値、下が最小値）
        var percentage = 1 - Math.Clamp(thumbPosition / trackHeight, 0, 1);
        var newValue = Minimum + (percentage * (Maximum - Minimum));

        Value = Math.Round(newValue, 1); // 小数点1桁に丸める
    }

    // タッチ位置がつまみ領域内かどうかを判定するメソッド
    // マージンを追加して、タッチしやすくする
    private bool IsPointInThumb(PointF point)
    {
        var centerX = Width / 2;
        var thumbY = GetThumbYPosition();

        // つまみの領域を定義（マージンを追加）
        var thumbRect = new RectF(
            (float)centerX - (ThumbWidth / 2) - THUMB_TOUCH_MARGIN,
            thumbY - (ThumbHeight / 2) - THUMB_TOUCH_MARGIN,
            ThumbWidth + (THUMB_TOUCH_MARGIN * 2),
            ThumbHeight + (THUMB_TOUCH_MARGIN * 2));

        // タッチ位置がつまみ領域内にあるかチェック
        return thumbRect.Contains(point);
    }

    // 現在の値に基づいてつまみのY座標を計算
    private float GetThumbYPosition()
    {
        var valuePercentage = (Value - Minimum) / (Maximum - Minimum);
        var trackTop = ThumbHeight / 2;
        var trackBottom = Height - (ThumbHeight / 2);
        var trackHeight = trackBottom - trackTop;

        // サムのY座標（上が最大値、下が最小値）
        return (float)(trackBottom - (valuePercentage * trackHeight));
    }

    // タッチイベントハンドラー
    private void OnStartInteraction(object? sender, TouchEventArgs e)
    {
        var touchPoint = e.Touches.First();

        if (IsPointInThumb(touchPoint))
        {
            // ノブ周辺がタッチされた場合、ドラッグモードを開始
            _isDragging = true;
            UpdateValue(touchPoint);
        }
        else
        {
            // ノブ以外の場所がタッチされた場合、その位置に値を設定するだけ
            UpdateValue(touchPoint);
            _isDragging = false;
        }
    }

    private void OnDragInteraction(object? sender, TouchEventArgs e)
    {
        // ドラッグモードの場合のみ値を更新
        if (_isDragging)
        {
            UpdateValue(e.Touches.First());
        }
    }

    private void OnEndInteraction(object? sender, TouchEventArgs e)
    {
        // ドラッグ終了時はドラッグモードをリセット
        _isDragging = false;
    }

    // プロパティ変更時の処理
    private static void OnSliderPropertyChanged(BindableObject bindable, object oldValue, object newValue)
    {
        // スライダーの再描画を行う
        ((VerticalMixerSlider)bindable).Invalidate();
    }

    private static void OnValueChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var slider = (VerticalMixerSlider)bindable;
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
        var slider = (VerticalMixerSlider)bindable;
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
internal class VerticalMixerSliderDrawable : IDrawable
{
    private readonly VerticalMixerSlider _slider;

    public VerticalMixerSliderDrawable(VerticalMixerSlider slider)
    {
        _slider = slider;
    }

    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        // トラックの中心X座標
        var centerX = dirtyRect.Width / 2;

        // スライダーの値に基づくサムの位置を計算
        var valuePercentage = (_slider.Value - _slider.Minimum) / (_slider.Maximum - _slider.Minimum);

        // トラックのY座標範囲（サムの半分の高さ分のマージンを取る）
        var trackTop = _slider.ThumbHeight / 2;
        var trackBottom = dirtyRect.Height - (_slider.ThumbHeight / 2);
        var trackHeight = trackBottom - trackTop;

        // サムのY座標（上が最大値、下が最小値）
        var thumbY = trackBottom - (float)(valuePercentage * trackHeight);

        // 目盛りを描画（オプション）
        if (_slider.HasTickMarks && _slider.TickMarkCount > 1)
        {
            DrawTickMarks(canvas, centerX, trackTop, trackBottom);
        }

        // トラックを描画（背景部分）
        canvas.StrokeSize = _slider.TrackWidth;
        canvas.StrokeColor = _slider.TrackColor;
        canvas.DrawLine(centerX, trackTop, centerX, trackBottom);

        // 進捗部分のトラックを描画（選択値までの部分）
        canvas.StrokeColor = _slider.ProgressColor;
        canvas.DrawLine(centerX, thumbY, centerX, trackBottom);

        // サム（つまみ）を描画
        DrawRectangularThumb(canvas, centerX, thumbY);
    }

    private void DrawTickMarks(ICanvas canvas, float centerX, float trackTop, float trackBottom)
    {
        canvas.StrokeColor = _slider.TickMarkColor;
        canvas.StrokeSize = 1;

        var trackHeight = trackBottom - trackTop;

        for (int i = 0; i < _slider.TickMarkCount; i++)
        {
            var tickPercentage = (float)i / (_slider.TickMarkCount - 1);
            var tickY = trackBottom - (tickPercentage * trackHeight);

            // 目盛り線を描画（左右に伸びるように）
            canvas.DrawLine(
                centerX - _slider.TickMarkLength,
                tickY,
                centerX + _slider.TickMarkLength,
                tickY);
        }
    }

    private void DrawRectangularThumb(ICanvas canvas, float centerX, float thumbY)
    {
        var halfWidth = _slider.ThumbWidth / 2;
        var halfHeight = _slider.ThumbHeight / 2;

        // 四角形のつまみを描画
        var rect = new RectF(
            centerX - halfWidth,
            thumbY - halfHeight,
            _slider.ThumbWidth,
            _slider.ThumbHeight);

        // 3D効果のための微妙な影をつける
        canvas.SetShadow(new SizeF(1, 1), 2, Colors.Black.WithAlpha(0.3f));

        // 背景を塗りつぶし
        canvas.FillColor = _slider.ThumbColor;
        canvas.FillRoundedRectangle(rect, 2);

        // 影をリセット
        canvas.SetShadow(new SizeF(0, 0), 0, Colors.Transparent);

        // 枠線を描画
        canvas.StrokeColor = _slider.ThumbBorderColor;
        canvas.StrokeSize = 1;
        canvas.DrawRoundedRectangle(rect, 2);

        // つまみの中心に横線を描画（目印として）
        canvas.StrokeColor = _slider.ThumbBorderColor;
        canvas.StrokeSize = 1.5f;
        canvas.DrawLine(
            centerX - (halfWidth * 0.6f),
            thumbY,
            centerX + (halfWidth * 0.6f),
            thumbY);
    }
}