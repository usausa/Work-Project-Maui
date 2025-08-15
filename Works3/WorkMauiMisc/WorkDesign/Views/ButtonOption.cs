using Microsoft.Maui.Platform;

namespace WorkDesign;

using Microsoft.Maui.Handlers;

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
