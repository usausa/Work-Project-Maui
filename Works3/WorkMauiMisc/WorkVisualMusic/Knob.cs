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

    public KnobView()
    {
        Drawable = new KnobDrawable(this);

        // パン操作（ドラッグ）の検知
        var pan = new PanGestureRecognizer();
        pan.PanUpdated += OnPanUpdated;
        this.GestureRecognizers.Add(pan);
    }

    private void OnPanUpdated(object sender, PanUpdatedEventArgs e)
    {
        //if (e.StatusType == GestureStatus.Running || e.StatusType == GestureStatus.Completed)
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
            if (knobAngle > 270) return; // 範囲外は無視

            double percent = knobAngle / 270.0;
            Value = Minimum + percent * (Maximum - Minimum);
        }
    }

    // 描画用クラス
    class KnobDrawable : IDrawable
    {
        KnobView _parent;
        public KnobDrawable(KnobView parent) { _parent = parent; }
        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            var centerX = dirtyRect.Center.X;
            var centerY = dirtyRect.Center.Y;
            var radius = Math.Min(dirtyRect.Width, dirtyRect.Height) / 2 - 10;

            // ノブ円
            canvas.FillColor = Colors.LightGray;
            canvas.FillCircle(centerX, centerY, (float)radius);

            // インジケーター
            double angle = 135 + 270 * ((_parent.Value - _parent.Minimum) / (_parent.Maximum - _parent.Minimum));
            double rad = Math.PI * angle / 180.0;
            float x2 = (float)(centerX + Math.Cos(rad) * (radius - 12));
            float y2 = (float)(centerY + Math.Sin(rad) * (radius - 12));

            canvas.StrokeColor = Colors.Orange;
            canvas.StrokeSize = 5;
            canvas.DrawLine(centerX, centerY, x2, y2);

            canvas.FontColor = Colors.Black;
            canvas.FontSize = 18;
            canvas.DrawString($"{_parent.Value:0.##}", centerX, centerY, HorizontalAlignment.Center);
        }
    }
}