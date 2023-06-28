using Android.Widget;

namespace WorkEffect.Behaviors;

public sealed class SelectAllTextBehavior : PlatformBehavior<InputView, EditText>
{
    protected override void OnAttachedTo(InputView bindable, EditText platformView)
    {
        base.OnAttachedTo(bindable, platformView);
        platformView.SetSelectAllOnFocus(true);
    }

    protected override void OnDetachedFrom(InputView bindable, EditText platformView)
    {
        platformView.SetSelectAllOnFocus(false);
        base.OnDetachedFrom(bindable, platformView);
    }
}
