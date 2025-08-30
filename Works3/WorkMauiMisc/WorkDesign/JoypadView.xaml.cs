namespace WorkDesign;

using Microsoft.Maui.Graphics;

public partial class JoypadView : ContentView
{
    public event EventHandler<(float x, float y)> JoystickChanged;
    public event EventHandler<string> ButtonPressed;

    private double _baseRadius = 60;
    private double _knobRadius = 30;

    public JoypadView()
    {
        InitializeComponent();

        JoystickBaseGraphics.Drawable = new JoystickKnobBackgroundDrawable();
        JoystickKnobView.Drawable = new JoystickKnobDrawable();

        var panGesture = new PanGestureRecognizer();
        panGesture.TouchPoints = 1;
        panGesture.PanUpdated += OnJoystickPanUpdated;
        JoystickBaseGrid.GestureRecognizers.Add(panGesture);

        ButtonA.Clicked += (_, __) => ButtonPressed?.Invoke(this, "A");
        ButtonB.Clicked += (_, __) => ButtonPressed?.Invoke(this, "B");
        ButtonX.Clicked += (_, __) => ButtonPressed?.Invoke(this, "X");
        ButtonY.Clicked += (_, __) => ButtonPressed?.Invoke(this, "Y");
    }

    private void OnJoystickPanUpdated(object sender, PanUpdatedEventArgs e)
    {
        switch (e.StatusType)
        {
            case GestureStatus.Started:
            case GestureStatus.Running:
                {
                    var x = e.TotalX;
                    var y = e.TotalY;
                    var maxDistance = _baseRadius - _knobRadius;
                    var distance = Math.Sqrt(x * x + y * y);
                    if (distance > maxDistance)
                    {
                        double scale = maxDistance / distance;
                        x *= scale;
                        y *= scale;
                    }
                    JoystickKnobView.TranslationX = x;
                    JoystickKnobView.TranslationY = y;
                    JoystickChanged?.Invoke(this, ((float)(x / maxDistance), (float)(y / maxDistance)));
                    break;
                }
            case GestureStatus.Completed:
            case GestureStatus.Canceled:
                {
                    JoystickKnobView.TranslationX = 0;
                    JoystickKnobView.TranslationY = 0;
                    JoystickChanged?.Invoke(this, (0, 0));
                    break;
                }
        }
    }

    // ボタン押下エフェクト: 小さく&暗く
    private void ButtonEffect(Button btn, bool pressed)
    {
        if (pressed)
        {
            btn.ScaleTo(0.85, 50, Easing.CubicOut);   // 小さく
            btn.FadeTo(0.7, 50, Easing.CubicOut);     // 少し暗く
        }
        else
        {
            btn.ScaleTo(1.0, 100, Easing.CubicOut);   // 元のサイズ
            btn.FadeTo(1.0, 100, Easing.CubicOut);    // 元の明るさ
        }
    }

    // イベントハンドラ
    private void OnButtonPressedA(object sender, EventArgs e) => ButtonEffect(ButtonA, true);
    private void OnButtonReleasedA(object sender, EventArgs e) => ButtonEffect(ButtonA, false);
    private void OnButtonPressedB(object sender, EventArgs e) => ButtonEffect(ButtonB, true);
    private void OnButtonReleasedB(object sender, EventArgs e) => ButtonEffect(ButtonB, false);
    private void OnButtonPressedX(object sender, EventArgs e) => ButtonEffect(ButtonX, true);
    private void OnButtonReleasedX(object sender, EventArgs e) => ButtonEffect(ButtonX, false);
    private void OnButtonPressedY(object sender, EventArgs e) => ButtonEffect(ButtonY, true);
    private void OnButtonReleasedY(object sender, EventArgs e) => ButtonEffect(ButtonY, false);

    // ジョイスティックベース（円形グラデーション）
    class JoystickBaseDrawable : IDrawable
    {
        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            canvas.SaveState();

            var grad = new RadialGradientPaint
            {
                Center = dirtyRect.Center,
                Radius = 60,
                GradientStops = new[]
                {
                    new PaintGradientStop(0.0f, Color.FromArgb("#222")),
                    new PaintGradientStop(1.0f, Color.FromArgb("#444"))
                }
            };
            canvas.SetFillPaint(grad, dirtyRect);
            canvas.FillCircle(dirtyRect.Center.X, dirtyRect.Center.Y, 60);

            // 外枠
            canvas.StrokeColor = Colors.White;
            canvas.StrokeSize = 2;
            canvas.DrawCircle(dirtyRect.Center.X, dirtyRect.Center.Y, 60);

            canvas.RestoreState();
        }
    }

    public class JoystickKnobBackgroundDrawable : IDrawable
    {
        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            canvas.SaveState();

            // "浅くへこんだ"印象のグラデーション
            // 中央が暗め（#444444）、周囲が明るめ（#BBBBBB）→凹みに見える
            var grad = new RadialGradientPaint
            {
                Center = dirtyRect.Center,
                Radius = dirtyRect.Height / 2,
                GradientStops = new[]
                {
                    new PaintGradientStop(0.0f, Color.FromArgb("#444444")), // 中央:濃いグレー
                    new PaintGradientStop(0.7f, Color.FromArgb("#888888")), // 中間:やや明るい
                    new PaintGradientStop(1.0f, Color.FromArgb("#BBBBBB"))  // 外側:明るいグレー
                }
            };
            canvas.SetFillPaint(grad, dirtyRect);
            canvas.FillCircle(dirtyRect.Center.X, dirtyRect.Center.Y, dirtyRect.Height / 2);

            canvas.RestoreState();
        }
    }

    public class JoystickKnobDrawable : IDrawable
    {
        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            canvas.SaveState();

            // "膨らんだ"印象のグラデーション
            // 中央が明るい（#A5D7FF）、外側が濃い青（#1976D2）→膨らんだ球の光沢感
            var grad = new RadialGradientPaint
            {
                Center = dirtyRect.Center,
                Radius = dirtyRect.Height / 2,
                GradientStops = new[]
                {
                    new PaintGradientStop(0.0f, Color.FromArgb("#A5D7FF")), // 中央:明るい青
                    new PaintGradientStop(0.5f, Color.FromArgb("#42A5F5")), // 中間:青
                    new PaintGradientStop(0.85f, Color.FromArgb("#1976D2")), // 外側:濃い青
                    new PaintGradientStop(1.0f, Color.FromArgb("#0D47A1"))   // 最外:さらに濃い青
                }
            };
            canvas.SetFillPaint(grad, dirtyRect);
            canvas.FillCircle(dirtyRect.Center.X, dirtyRect.Center.Y, dirtyRect.Height / 2);

            canvas.RestoreState();
        }
    }
}