﻿namespace OverlayExample;

public interface ILoading : IDisposable
{
    void Update(string text);
}

public interface IProgress : IDisposable
{
    void Update(double value);
}

public interface IDialog
{
    IDisposable Lock();

    ILoading Loading(string text = "");

    IProgress Progress();
}

public sealed class Dialog : IDialog
{
    public IDisposable Lock()
    {
        var window = Application.Current!.MainPage!.GetParentWindow();
        return new LockOverlay(window);
    }

    public ILoading Loading(string text = "")
    {
        var window = Application.Current!.MainPage!.GetParentWindow();
        return new LoadingOverlay(window, text);
    }

    public IProgress Progress()
    {
        var window = Application.Current!.MainPage!.GetParentWindow();
        return new ProgressOverlay(window);
    }
}

internal sealed class LockOverlay : WindowOverlay, IDisposable
{
    public LockOverlay(IWindow window)
        : base(window)
    {
        AddWindowElement(new ElementOverlay());
        EnableDrawableTouchHandling = true;

        window.AddOverlay(this);
    }

    public void Dispose()
    {
        Window.RemoveOverlay(this);
    }

    private class ElementOverlay : IWindowOverlayElement
    {
        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            canvas.FillColor = Color.FromRgba(0, 0, 0, 128);
            canvas.FillRectangle(dirtyRect);
        }

        public bool Contains(Point point) => true;
    }
}

internal sealed class LoadingOverlay : WindowOverlay, ILoading
{
    private readonly ElementOverlay overlay;

    public LoadingOverlay(IWindow window, string text)
        : base(window)
    {
        overlay = new ElementOverlay { Text = text };
        AddWindowElement(overlay);
        EnableDrawableTouchHandling = true;

        window.AddOverlay(this);
    }

    public void Dispose()
    {
        Window.RemoveOverlay(this);
    }

    public void Update(string text)
    {
        overlay.Text = text;
        Invalidate();
    }

    private class ElementOverlay : IWindowOverlayElement
    {
        public string Text { get; set; } = string.Empty;

        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            canvas.FillColor = Color.FromRgba(0, 0, 0, 128);
            canvas.FillRectangle(dirtyRect);

            var messageRect = new RectF(32, (dirtyRect.Height / 2) - 32, dirtyRect.Width - 64, 64);

            canvas.FillColor = Color.FromRgba(0, 0, 0, 128);
            canvas.FillRoundedRectangle(messageRect, 8);

            canvas.FontColor = Colors.White;
            canvas.FontSize = 24;
            canvas.DrawString(Text, messageRect, HorizontalAlignment.Center, VerticalAlignment.Center);
        }

        public bool Contains(Point point) => true;
    }
}

internal sealed class ProgressOverlay : WindowOverlay, IProgress
{
    private readonly ElementOverlay overlay;

    public ProgressOverlay(IWindow window)
        : base(window)
    {
        overlay = new ElementOverlay();
        AddWindowElement(overlay);
        EnableDrawableTouchHandling = true;

        window.AddOverlay(this);
    }

    public void Dispose()
    {
        Window.RemoveOverlay(this);
    }

    public void Update(double value)
    {
        overlay.Value = value switch
        {
            > 100 => 100,
            < 0 => 0,
            _ => value
        };
        Invalidate();
    }

    private class ElementOverlay : IWindowOverlayElement
    {
        public double Value { get; set; }

        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            canvas.FillColor = Color.FromRgba(0, 0, 0, 128);
            canvas.FillRectangle(dirtyRect);

            canvas.FillColor = Color.FromRgba(0, 0, 0, 128);
            canvas.FillRoundedRectangle(new RectF((dirtyRect.Width / 2) - 80, (dirtyRect.Height / 2) - 80, 160, 160), 16);

            var arcRect = new RectF((dirtyRect.Width / 2) - 64, (dirtyRect.Height / 2) - 64, 128, 128);

            canvas.StrokeSize = 8;
            canvas.StrokeColor = Colors.Gray;
            canvas.DrawArc(arcRect, 0, 360, false, false);

            canvas.StrokeColor = Colors.White;
            canvas.DrawArc(arcRect, 90, 90 - (int)(360 * Value / 100), true, false);

            canvas.FontColor = Colors.White;
            canvas.FontSize = 24;
            canvas.DrawString($"{Value:F1}%", arcRect, HorizontalAlignment.Center, VerticalAlignment.Center);
        }

        public bool Contains(Point point) => true;
    }
}
