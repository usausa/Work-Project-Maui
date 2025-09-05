using System.Diagnostics;

namespace WorkVisualMusic;
using Microsoft.Maui.Graphics;

using System;

public class KnobView : GraphicsView
{
    public static readonly BindableProperty ValueProperty =
        BindableProperty.Create(nameof(Value), typeof(double), typeof(KnobView), 0.0, propertyChanged: (b, o, n) => ((KnobView)b).Invalidate());

    public static readonly BindableProperty MinimumProperty =
        BindableProperty.Create(nameof(Minimum), typeof(double), typeof(KnobView), 0.0);

    public static readonly BindableProperty MaximumProperty =
        BindableProperty.Create(nameof(Maximum), typeof(double), typeof(KnobView), 1.0);

    public static readonly BindableProperty IndicatorColorProperty =
        BindableProperty.Create(nameof(IndicatorColor), typeof(Color), typeof(KnobView), Colors.Orange, propertyChanged: (b, o, n) => ((KnobView)b).Invalidate());

    public static readonly BindableProperty TitleProperty =
        BindableProperty.Create(nameof(Title), typeof(string), typeof(KnobView), string.Empty, propertyChanged: (b, o, n) => ((KnobView)b).Invalidate());

    public double Value
    {
        get => (double)GetValue(ValueProperty);
        set => SetValue(ValueProperty, Math.Clamp(value, Minimum, Maximum));
    }

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

    public Color IndicatorColor
    {
        get => (Color)GetValue(IndicatorColorProperty);
        set => SetValue(IndicatorColorProperty, value);
    }

    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public KnobView()
    {
        Drawable = new KnobDrawable(this);

        // パン操作（ドラッグ）の検知
        var pan = new PanGestureRecognizer();
        pan.PanUpdated += OnPanUpdated;
        this.GestureRecognizers.Add(pan);
    }

    private void OnPanUpdated(object? sender, PanUpdatedEventArgs e)
    {
        if (e.StatusType == GestureStatus.Running)
        {
            var center = new Point(Width / 2, Height / 2);
            var touchPoint = new Point(e.TotalX + center.X, e.TotalY + center.Y);

            // 角度計算
            var dx = touchPoint.X - center.X;
            var dy = touchPoint.Y - center.Y;
            var angle = Math.Atan2(dy, dx) * 180 / Math.PI;
            angle = (angle + 360) % 360;

            // 135度～405度の範囲をノブの可動域にする
            double knobAngle = angle - 135;
            if (knobAngle < 0) knobAngle += 360;
            if (knobAngle > 270) knobAngle = 270;
            // TODO 用補正、

            double percent = knobAngle / 270.0;
            Debug.WriteLine(percent);
            Value = Minimum + percent * (Maximum - Minimum);
        }
    }

    // 描画用クラス
    class KnobDrawable : IDrawable
    {
        private const float StartAngle = 135;
        private const float SweepAngle = 270;
        private const float IndicatorSize = 5; // 小さな円のサイズ（参考画像に合わせて調整）
        private const float ArcThicknessRatio = 0.08f; // 円弧の太さの比率
        private const float MarginRatio = 0.1f; // 円弧とノブの間のマージン比率
        private const float IndicatorDistanceRatio = 0.9f; // ノブの中心からインジケーターまでの距離の比率

        KnobView _parent;
        public KnobDrawable(KnobView parent) { _parent = parent; }

        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            var centerX = dirtyRect.Center.X;
            var centerY = dirtyRect.Center.Y;

            // タイトルを描画（コントロールの上部）
            if (!string.IsNullOrEmpty(_parent.Title))
            {
                canvas.FontColor = Colors.Black;
                canvas.FontSize = 14;
                canvas.DrawString(_parent.Title, centerX, 10, HorizontalAlignment.Center);
            }

            var outerRadius = Math.Min(dirtyRect.Width, dirtyRect.Height) / 2 - 15; // 余白を確保
            var arcThickness = outerRadius * ArcThicknessRatio; // 円弧の太さ

            // 円弧とノブの間のマージンを設定
            var margin = outerRadius * MarginRatio;

            // ノブの半径を計算 (円弧の内側 - マージン)
            var knobRadius = outerRadius - arcThickness - margin;

            // 全体の範囲を灰色の円弧で描画
            canvas.StrokeColor = Colors.LightGray;
            canvas.StrokeSize = arcThickness;
            canvas.DrawArc(
                centerX - outerRadius + arcThickness / 2,
                centerY - outerRadius + arcThickness / 2,
                outerRadius * 2 - arcThickness,
                outerRadius * 2 - arcThickness,
                StartAngle,
                SweepAngle,
                false,
                false);

            // 値の範囲を色付きの円弧で描画
            var valuePercent = (_parent.Value - _parent.Minimum) / (_parent.Maximum - _parent.Minimum);
            var valueSweepAngle = (float)(SweepAngle * valuePercent);

            canvas.StrokeColor = _parent.IndicatorColor;
            canvas.StrokeSize = arcThickness;
            canvas.DrawArc(
                centerX - outerRadius + arcThickness / 2,
                centerY - outerRadius + arcThickness / 2,
                outerRadius * 2 - arcThickness,
                outerRadius * 2 - arcThickness,
                StartAngle,
                valueSweepAngle,
                false,
                false);

            // ノブ円 (円弧より内側にマージンを付けて描画)
            canvas.FillColor = Colors.DarkGray; // 参考画像に合わせて暗い灰色に変更
            canvas.FillCircle(centerX, centerY, (float)knobRadius);

            // 円形インジケーターをノブの内側に描画
            double angle = StartAngle + valueSweepAngle;
            double rad = Math.PI * angle / 180.0;
            // インジケーターの位置をノブの内側の縁近くに調整
            float indicatorDistance = (float)(knobRadius * IndicatorDistanceRatio);
            float indicatorX = (float)(centerX + Math.Cos(rad) * indicatorDistance);
            float indicatorY = (float)(centerY + Math.Sin(rad) * indicatorDistance);

            canvas.FillColor = _parent.IndicatorColor;
            canvas.FillCircle(indicatorX, indicatorY, IndicatorSize);

            // 値の表示（整数値で表示）- ノブの円の下に描画
            canvas.FontColor = Colors.White; // 参考画像に合わせて色を変更
            canvas.FontSize = 16;
            // ノブの下端から少し下の位置に描画
            var valueTextY = centerY + knobRadius + 20;
            canvas.DrawString($"{(int)_parent.Value}", centerX, valueTextY, HorizontalAlignment.Center);
        }
    }
}