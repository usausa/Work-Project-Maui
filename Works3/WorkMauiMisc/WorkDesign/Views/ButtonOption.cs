using Microsoft.Maui.Platform;

namespace WorkDesign;

using Microsoft.Maui.Handlers;

using Smart.Maui.Interactivity;

public static partial class ButtonOption
{
    public static readonly BindableProperty DisableRippleEffectProperty = BindableProperty.CreateAttached(
        "DisableRippleEffect",
        typeof(bool),
        typeof(ButtonOption),
        false);

    public static bool GetDisableRippleEffect(BindableObject bindable) => (bool)bindable.GetValue(DisableRippleEffectProperty);

    public static void SetDisableRippleEffect(BindableObject bindable, bool value) => bindable.SetValue(DisableRippleEffectProperty, value);

    public static partial void UseCustomMapper();
}

public static partial class ButtonOption
{
    public static partial void UseCustomMapper()
    {
        ButtonHandler.Mapper.AppendToMapping(ButtonOption.DisableRippleEffectProperty.PropertyName, UpdateDisableRippleEffect);
    }

    private static void UpdateDisableRippleEffect(IButtonHandler handler, IButton view)
    {
#if ANDROID
        if ((view is Button button) && GetDisableRippleEffect(button))
        {
            var color = button.BackgroundColor.ToPlatform();
            handler.PlatformView.SetBackgroundColor(color);
        }
#endif
    }

    // ------------------------------------------------------------
    // Pressed
    // ------------------------------------------------------------

    public static readonly BindableProperty IsPressedProperty = BindableProperty.CreateAttached(
        "IsPressed",
        typeof(bool),
        typeof(ButtonOption),
        false,
        defaultBindingMode: BindingMode.OneWayToSource);

    public static bool GetIsPressed(BindableObject obj) =>
        (bool)obj.GetValue(IsPressedProperty);

    public static void SetIsPressed(BindableObject obj, bool value) =>
        obj.SetValue(IsPressedProperty, value);

    public static readonly BindableProperty PressBindProperty = BindableProperty.CreateAttached(
        "PressBind",
        typeof(bool),
        typeof(ButtonOption),
        defaultValue: false,
        propertyChanged: OnPressBindChanged);

    public static bool GetPressBind(BindableObject obj) =>
        (bool)obj.GetValue(PressBindProperty);

    public static void SetPressBind(BindableObject obj, bool value) =>
        obj.SetValue(PressBindProperty, value);

    private static void OnPressBindChanged(BindableObject bindable, object? oldValue, object? newValue)
    {
        if (bindable is not Button view)
        {
            return;
        }

        if (oldValue is not null)
        {
            var behavior = view.Behaviors.FirstOrDefault(static x => x is PressBindBehavior);
            if (behavior is not null)
            {
                view.Behaviors.Remove(behavior);
            }
        }

        if (newValue is not null)
        {
            view.Behaviors.Add(new PressBindBehavior());
        }
    }

    private sealed class PressBindBehavior : BehaviorBase<Button>
    {
        protected override void OnAttachedTo(Button bindable)
        {
            base.OnAttachedTo(bindable);

            bindable.Pressed += OnPressed;
            bindable.Released += OnReleased;
            bindable.Unfocused += OnReleased;
        }

        protected override void OnDetachingFrom(Button bindable)
        {
            bindable.Pressed -= OnPressed;
            bindable.Released -= OnReleased;
            bindable.Unfocused -= OnReleased;

            base.OnDetachingFrom(bindable);
        }

        private void OnPressed(object? sender, EventArgs e)
        {
            var button = AssociatedObject;
            if (button is not null)
            {
                SetIsPressed(button, true);
            }
        }

        private void OnReleased(object? sender, EventArgs e)
        {
            var button = AssociatedObject;
            if (button is not null)
            {
                SetIsPressed(button, false);
            }
        }
    }

    public static readonly BindableProperty PressEffectProperty = BindableProperty.CreateAttached(
        "PressEffect",
        typeof(bool),
        typeof(ButtonOption),
        defaultValue: false,
        propertyChanged: OnPressEffectChanged);

    public static bool GetPressEffect(BindableObject obj) =>
        (bool)obj.GetValue(PressEffectProperty);

    public static void SetPressEffect(BindableObject obj, bool value) =>
        obj.SetValue(PressEffectProperty, value);

    private static void OnPressEffectChanged(BindableObject bindable, object? oldValue, object? newValue)
    {
        if (bindable is not Button view)
        {
            return;
        }

        if (oldValue is not null)
        {
            var behavior = view.Behaviors.FirstOrDefault(static x => x is PressEffectBehavior);
            if (behavior is not null)
            {
                view.Behaviors.Remove(behavior);
            }
        }

        if (newValue is not null)
        {
            view.Behaviors.Add(new PressEffectBehavior());
        }
    }

    private sealed class PressEffectBehavior : BehaviorBase<Button>
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
            if (sender is Button button)
            {
                button.ScaleTo(0.9, 50, Easing.CubicOut);
                button.FadeTo(0.8, 50, Easing.CubicOut);
            }
        }

        private void OnButtonReleased(object? sender, EventArgs e)
        {
            if (sender is Button button)
            {
                button.ScaleTo(1.0, 100, Easing.CubicOut);
                button.FadeTo(1.0, 100, Easing.CubicOut);
            }
        }
    }
}

public static partial class ScrollViewOption
{
    public static readonly BindableProperty DisableOverScrollProperty = BindableProperty.CreateAttached(
        "DisableOverScroll",
        typeof(bool),
        typeof(ScrollViewOption),
        false);

    public static bool GetDisableOverScroll(BindableObject bindable) => (bool)bindable.GetValue(DisableOverScrollProperty);

    public static void SetDisableOverScroll(BindableObject bindable, bool value) => bindable.SetValue(DisableOverScrollProperty, value);

    public static partial void UseCustomMapper();
}

public static partial class ScrollViewOption
{
    public static partial void UseCustomMapper()
    {
        ViewHandler.ViewMapper.AppendToMapping(ScrollViewOption.DisableOverScrollProperty.PropertyName, Method);
    }

    private static void Method(IViewHandler handler, IView view)
    {
#if ANDROID
        if (view is BindableObject bindable && GetDisableOverScroll(bindable) && handler.PlatformView is Android.Views.View v)
        {
            v.OverScrollMode = Android.Views.OverScrollMode.Never;
        }
#endif
    }
}

internal static class AppHostBuilderExtensions
{
    public static MauiAppBuilder ConfigureCustomBehaviors(this MauiAppBuilder builder)
    {
        ButtonOption.UseCustomMapper();
        ScrollViewOption.UseCustomMapper();

        return builder;
    }
}
