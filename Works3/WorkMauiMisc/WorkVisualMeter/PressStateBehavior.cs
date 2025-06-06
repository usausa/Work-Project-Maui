using Smart.Maui.Interactivity;

namespace WorkVisualMeter;

public sealed class PressStateBehavior : BehaviorBase<Button>
{
    public static readonly BindableProperty IsPressedProperty =
        BindableProperty.Create(
            nameof(IsPressed),
            typeof(bool),
            typeof(PressStateBehavior),
            false,
            defaultBindingMode: BindingMode.OneWayToSource);

    public bool IsPressed
    {
        get => (bool)GetValue(IsPressedProperty);
        set => SetValue(IsPressedProperty, value);
    }

    protected override void OnAttachedTo(Button bindable)
    {
        base.OnAttachedTo(bindable);

        bindable.Pressed += OnButtonPressed;
        bindable.Released += OnButtonReleased;
    }

    protected override void OnDetachingFrom(Button bindable)
    {
        bindable.Pressed -= OnButtonPressed;
        bindable.Released -= OnButtonReleased;

        base.OnDetachingFrom(bindable);
    }

    void OnButtonPressed(object? sender, EventArgs e)
    {
        IsPressed = true;
    }

    void OnButtonReleased(object? sender, EventArgs e)
    {
        IsPressed = false;
    }
}
