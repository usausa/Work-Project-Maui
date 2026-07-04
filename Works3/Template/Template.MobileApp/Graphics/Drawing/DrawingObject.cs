namespace Template.MobileApp.Graphics.Drawing;

public interface IDrawingObject : IDrawable
{
    void Attach(DrawingControl view);

    void Detach();
}

#pragma warning disable CA1033
public abstract class DrawingObject : IDrawingObject
{
    private DrawingControl? control;

    void IDrawingObject.Attach(DrawingControl view)
    {
        control = view;
    }

    void IDrawingObject.Detach()
    {
        control = null;
    }

    public void Invalidate()
    {
        control?.Invalidate();
    }

    public void SafeInvalidate()
    {
        if (control is not null)
        {
            if (control.Dispatcher.IsDispatchRequired)
            {
                control.Dispatcher.Dispatch(control.Invalidate);
            }
            else
            {
                control.Invalidate();
            }
        }
    }

    void IDrawable.Draw(ICanvas canvas, RectF dirtyRect)
    {
        OnDraw(canvas, dirtyRect);
    }

    protected abstract void OnDraw(ICanvas canvas, RectF dirtyRect);
}
#pragma warning restore CA1033
