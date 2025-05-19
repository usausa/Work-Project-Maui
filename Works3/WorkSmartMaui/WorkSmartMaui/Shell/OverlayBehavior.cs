namespace WorkSmartMaui.Shell;

using System;
using System.Timers;

public static class OverlayBehavior
{
    public static readonly BindableProperty IsBusyProperty = BindableProperty.CreateAttached(
        "IsBusy",
        typeof(bool),
        typeof(OverlayBehavior),
        false,
        propertyChanged: HandleIsBusyChanged);

    public static bool GetIsBusy(BindableObject obj)
    {
        return (bool)obj.GetValue(IsBusyProperty);
    }

    public static void SetIsBusy(BindableObject obj, bool value)
    {
        obj.SetValue(IsBusyProperty, value);
    }

    private static void HandleIsBusyChanged(BindableObject bindable, object? oldValue, object? newValue)
    {
        if (oldValue == newValue)
        {
            return;
        }

        if (newValue is true)
        {
            if (overlay is null)
            {
                var window = Application.Current!.Windows[0];
                overlay = new LoadingOverlay(window);
            }
            overlay.Show();
        }
        else
        {
            overlay?.Hide();
        }
    }

    private static LoadingOverlay? overlay;

    private sealed class LoadingOverlay : WindowOverlay, IDisposable
    {
        private readonly ElementOverlay element;

        private Timer? timer;

        public LoadingOverlay(IWindow window)
            : base(window)
        {
            element = new ElementOverlay();
            AddWindowElement(element);
            EnableDrawableTouchHandling = true;
        }

        public void Dispose()
        {
            timer?.Dispose();
        }

        public void Show()
        {
            Window.AddOverlay(this);
            element.Progress = 0;
            if (timer is null)
            {
                timer = new Timer(16);
                timer.Elapsed += TimerOnElapsed;
            }
            timer.Start();
        }

        private void TimerOnElapsed(object? sender, ElapsedEventArgs e)
        {
            element.Progress += 0.01f;
            if (element.Progress > 1f)
            {
                element.Progress = 0;
            }
            MainThread.BeginInvokeOnMainThread(Invalidate);
        }

        public void Hide()
        {
            timer?.Stop();
            Window.RemoveOverlay(this);
        }
    }


    private sealed class ElementOverlay : IWindowOverlayElement
    {
        public float Progress { get; set; }

        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            // Background
            canvas.FillColor = new(255, 255, 255, 64);
            canvas.FillRectangle(dirtyRect);

            var size = Math.Min(dirtyRect.Width, dirtyRect.Height) * 0.8f;
            var cx = dirtyRect.Center.X;
            var cy = dirtyRect.Center.Y;
            var radius = size / 2;

            // Back circle
            canvas.StrokeColor = Colors.LightGray;
            canvas.StrokeSize = 8;
            canvas.DrawCircle(cx, cy, radius);

            // Loading circle
            canvas.StrokeColor = Colors.Blue;
            canvas.StrokeSize = 8;
            var sweepAngle = 270;
            var startAngle = (Progress * 360) % 360;
            canvas.DrawArc(
                cx - radius, cy - radius,
                radius * 2, radius * 2,
                startAngle, sweepAngle,
                false, false);
        }

        public bool Contains(Point point) => true;
    }
}
