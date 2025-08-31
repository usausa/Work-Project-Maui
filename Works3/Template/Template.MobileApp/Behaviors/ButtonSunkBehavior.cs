namespace Template.MobileApp.Behaviors;

using Smart.Maui.Interactivity;

public sealed class ButtonSunkBehavior : BehaviorBase<Button>
{
    protected override void OnAttachedTo(Button bindable)
    {
        base.OnAttachedTo(bindable);

        bindable.Pressed += OnButtonPressed;
        bindable.Released += OnButtonReleased;
    }

    protected override void OnDetachingFrom(Button bindable)
    {
        base.OnDetachingFrom(bindable);

        bindable.Pressed -= OnButtonPressed;
        bindable.Released -= OnButtonReleased;
    }

    private void OnButtonPressed(object? sender, EventArgs e)
    {
        if (AssociatedObject is null)
        {
            return;
        }

        AssociatedObject.ScaleTo(0.9, 50, Easing.CubicOut);
        AssociatedObject.FadeTo(0.8, 50, Easing.CubicOut);
    }

    private void OnButtonReleased(object? sender, EventArgs e)
    {
        if (AssociatedObject is null)
        {
            return;
        }

        AssociatedObject.ScaleTo(1.0, 100, Easing.CubicOut);
        AssociatedObject.FadeTo(1.0, 100, Easing.CubicOut);
    }
}
