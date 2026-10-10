namespace Template.MobileApp.Behaviors;

public static partial class TouchOption
{
    public static partial void UseCustomMapper(BehaviorOptions options);

    // ------------------------------------------------------------------ Ripple

    // 押したときの波紋 (要素を増やさずにビューの前景に描く)
    public static readonly BindableProperty RippleProperty = BindableProperty.CreateAttached(
        "Ripple",
        typeof(bool),
        typeof(TouchOption),
        false);

    public static bool GetRipple(BindableObject bindable) => (bool)bindable.GetValue(RippleProperty);

    public static void SetRipple(BindableObject bindable, bool value) => bindable.SetValue(RippleProperty, value);

    // ------------------------------------------------------------------ ClickCommand

    // 押して離したときに実行する (要素を増やさずに標準のクリックで受ける)
    public static readonly BindableProperty ClickCommandProperty = BindableProperty.CreateAttached(
        "ClickCommand",
        typeof(ICommand),
        typeof(TouchOption),
        null);

    public static ICommand? GetClickCommand(BindableObject bindable) => (ICommand?)bindable.GetValue(ClickCommandProperty);

    public static void SetClickCommand(BindableObject bindable, ICommand? value) => bindable.SetValue(ClickCommandProperty, value);

    public static readonly BindableProperty ClickCommandParameterProperty = BindableProperty.CreateAttached(
        "ClickCommandParameter",
        typeof(object),
        typeof(TouchOption),
        null);

    public static object? GetClickCommandParameter(BindableObject bindable) => bindable.GetValue(ClickCommandParameterProperty);

    public static void SetClickCommandParameter(BindableObject bindable, object? value) => bindable.SetValue(ClickCommandParameterProperty, value);
}
