namespace Template.MobileApp.Input;

using Smart.Maui.Interactivity;

public sealed class InputControlBehavior : BehaviorBase<Page>, IInputHandler
{
    protected override void OnAttachedTo(Page bindable)
    {
        base.OnAttachedTo(bindable);
        bindable.Appearing += BindableOnAppearing;
        bindable.Disappearing += BindableOnDisappearing;
    }

    protected override void OnDetachingFrom(Page bindable)
    {
        bindable.Appearing -= BindableOnAppearing;
        bindable.Disappearing -= BindableOnDisappearing;
        base.OnDetachingFrom(bindable);
    }

    private void BindableOnAppearing(object? sender, EventArgs e)
    {
        InputManager.Default.PushHandler(this);
    }

    private void BindableOnDisappearing(object? sender, EventArgs e)
    {
        InputManager.Default.PopHandler(this);
    }

    public bool Handle(KeyCode key)
    {
        if ((AssociatedObject is null) || !AssociatedObject.IsEnabled)
        {
            return false;
        }

        // TODO
        return false;
    }

    //public VisualElement? FindFocused()
    //{
    //    // TODO
    //    return null;
    //}
}
