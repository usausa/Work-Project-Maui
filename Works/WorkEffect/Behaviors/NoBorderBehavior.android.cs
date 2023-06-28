using Android.Graphics.Drawables;

namespace WorkEffect.Behaviors;

using Android.Widget;

public sealed class NoBorderBehavior : PlatformBehavior<InputView, EditText>
{
    private Drawable? originalDrawable;

    protected override void OnAttachedTo(InputView bindable, EditText platformView)
    {
        base.OnAttachedTo(bindable, platformView);

        originalDrawable = platformView.Background;
        platformView.Background = null;
    }

    protected override void OnDetachedFrom(InputView bindable, EditText platformView)
    {
        platformView.Background = originalDrawable;
        base.OnDetachedFrom(bindable, platformView);
    }
}
