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

    //--------------------------------------------------------------------------------
    // Animation
    //--------------------------------------------------------------------------------

    // 単発アニメーションの共通機構。値を Easing 付きで進めて毎フレーム再描画し、
    // 完走時のみ completed を呼ぶ (中断時は呼ばない)。同名で再開始すると前の実行は中断される。
    // ChartDrawing の自前タイマー(定周期)や SceneObject の無限ループとは別の、
    // 「1回走って完了を通知する」型のアニメーションに使う。
    protected void AnimateValue(string name, double start, double end, uint length, Easing easing, Action<double> frame, Action? completed = null)
    {
        if (control is null)
        {
            frame(end);
            completed?.Invoke();
            return;
        }

        control.Animate(
            name,
            v =>
            {
                frame(v);
                Invalidate();
            },
            start,
            end,
            16,
            length,
            easing,
            (_, cancelled) =>
            {
                if (!cancelled)
                {
                    completed?.Invoke();
                }
            });
    }

    protected void AbortAnimation(string name) => control?.AbortAnimation(name);

    protected bool AnimationIsRunning(string name) => (control is not null) && control.AnimationIsRunning(name);

    void IDrawable.Draw(ICanvas canvas, RectF dirtyRect)
    {
        OnDraw(canvas, dirtyRect);
    }

    protected abstract void OnDraw(ICanvas canvas, RectF dirtyRect);
}
#pragma warning restore CA1033
