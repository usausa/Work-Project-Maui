namespace Template.MobileApp.Behaviors;

public static class AnimationOption
{
    private const string PulseAnimationName = "AnimationOptionPulse";
    private const string BounceAnimationName = "AnimationOptionBounce";
    private const string FadeInAnimationName = "AnimationOptionFadeIn";
    private const string HighlightAnimationName = "AnimationOptionHighlight";
    private const string FlashAnimationName = "AnimationOptionFlash";

    // ------------------------------------------------------------------ Pulse

    public static readonly BindableProperty PulseProperty = BindableProperty.CreateAttached(
        "Pulse",
        typeof(bool),
        typeof(AnimationOption),
        false,
        propertyChanged: OnPulseChanged);

    public static bool GetPulse(BindableObject bindable) => (bool)bindable.GetValue(PulseProperty);

    public static void SetPulse(BindableObject bindable, bool value) => bindable.SetValue(PulseProperty, value);

    private static void OnPulseChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is not VisualElement element)
        {
            return;
        }

        if ((bool)newValue)
        {
            element.Loaded += OnPulseLoaded;
            element.Unloaded += OnPulseUnloaded;
            if (element.IsLoaded)
            {
                StartPulse(element);
            }
        }
        else
        {
            element.Loaded -= OnPulseLoaded;
            element.Unloaded -= OnPulseUnloaded;
            element.AbortAnimation(PulseAnimationName);
        }
    }

    private static void OnPulseLoaded(object? sender, EventArgs e)
    {
        if (sender is VisualElement element)
        {
            StartPulse(element);
        }
    }

    private static void OnPulseUnloaded(object? sender, EventArgs e)
    {
        if (sender is VisualElement element)
        {
            element.AbortAnimation(PulseAnimationName);
        }
    }

    private static void StartPulse(VisualElement element)
    {
        element.AbortAnimation(PulseAnimationName);

        // 正弦カーブで 1.0 → 1.05 → 1.0 を繰り返す
        element.Animate(
            PulseAnimationName,
            v => element.Scale = 1.0 + (0.05 * Math.Sin(v * Math.PI)),
            16,
            3000,
            Easing.Linear,
            repeat: () => GetPulse(element));
    }

    // ------------------------------------------------------------------ Bounce

    public static readonly BindableProperty BounceTriggerProperty = BindableProperty.CreateAttached(
        "BounceTrigger",
        typeof(object),
        typeof(AnimationOption),
        null,
        propertyChanged: OnBounceTriggerChanged);

    public static object? GetBounceTrigger(BindableObject bindable) => bindable.GetValue(BounceTriggerProperty);

    public static void SetBounceTrigger(BindableObject bindable, object? value) => bindable.SetValue(BounceTriggerProperty, value);

    public static readonly BindableProperty BounceValueProperty = BindableProperty.CreateAttached(
        "BounceValue",
        typeof(object),
        typeof(AnimationOption),
        null);

    public static object? GetBounceValue(BindableObject bindable) => bindable.GetValue(BounceValueProperty);

    public static void SetBounceValue(BindableObject bindable, object? value) => bindable.SetValue(BounceValueProperty, value);

    private static void OnBounceTriggerChanged(BindableObject bindable, object? oldValue, object? newValue)
    {
        // 初回バインドでは跳ねない
        if ((bindable is not VisualElement element) || (oldValue is null) || (newValue is null))
        {
            return;
        }

        // BounceValue 指定時は一致した要素のみ跳ねる
        var expected = GetBounceValue(bindable);
        if ((expected is null) || Equals(newValue, expected))
        {
            StartBounce(element);
        }
    }

    private static void StartBounce(VisualElement element)
    {
        element.AbortAnimation(BounceAnimationName);

        // 正弦カーブで 1.0 → 1.15 → 1.0 と跳ねる
        element.Animate(
            BounceAnimationName,
            v => element.Scale = 1.0 + (0.15 * Math.Sin(v * Math.PI)),
            16,
            300,
            Easing.CubicOut,
            (_, _) => element.Scale = 1.0);
    }

    // ------------------------------------------------------------------ FadeIn

    public static readonly BindableProperty FadeInTriggerProperty = BindableProperty.CreateAttached(
        "FadeInTrigger",
        typeof(object),
        typeof(AnimationOption),
        null,
        propertyChanged: OnFadeInTriggerChanged);

    public static object? GetFadeInTrigger(BindableObject bindable) => bindable.GetValue(FadeInTriggerProperty);

    public static void SetFadeInTrigger(BindableObject bindable, object? value) => bindable.SetValue(FadeInTriggerProperty, value);

    private static void OnFadeInTriggerChanged(BindableObject bindable, object? oldValue, object? newValue)
    {
        // 初回バインドではフェードしない
        if ((bindable is not VisualElement element) || (oldValue is null) || (newValue is null))
        {
            return;
        }

        element.AbortAnimation(FadeInAnimationName);
        element.Opacity = 0;

        element.Animate(
            FadeInAnimationName,
            v => element.Opacity = v,
            16,
            250,
            Easing.CubicOut,
            (_, _) => element.Opacity = 1.0);
    }

    // ------------------------------------------------------------------ Flash

    public static readonly BindableProperty FlashTriggerProperty = BindableProperty.CreateAttached(
        "FlashTrigger",
        typeof(object),
        typeof(AnimationOption),
        null,
        propertyChanged: OnFlashTriggerChanged);

    public static object? GetFlashTrigger(BindableObject bindable) => bindable.GetValue(FlashTriggerProperty);

    public static void SetFlashTrigger(BindableObject bindable, object? value) => bindable.SetValue(FlashTriggerProperty, value);

    private static void OnFlashTriggerChanged(BindableObject bindable, object? oldValue, object? newValue)
    {
        // 初回バインドでは光らせない
        if ((bindable is not VisualElement element) || (oldValue is null) || (newValue is null))
        {
            return;
        }

        element.AbortAnimation(FlashAnimationName);
        element.Opacity = 1;

        element.Animate(
            FlashAnimationName,
            v => element.Opacity = 1 - v,
            16,
            300,
            Easing.CubicOut,
            (_, _) => element.Opacity = 0);
    }

    // ------------------------------------------------------------------ Highlight

    public static readonly BindableProperty HighlightTriggerProperty = BindableProperty.CreateAttached(
        "HighlightTrigger",
        typeof(object),
        typeof(AnimationOption),
        null,
        propertyChanged: OnHighlightTriggerChanged);

    public static object? GetHighlightTrigger(BindableObject bindable) => bindable.GetValue(HighlightTriggerProperty);

    public static void SetHighlightTrigger(BindableObject bindable, object? value) => bindable.SetValue(HighlightTriggerProperty, value);

    public static readonly BindableProperty HighlightColorProperty = BindableProperty.CreateAttached(
        "HighlightColor",
        typeof(Color),
        typeof(AnimationOption),
        null);

    public static Color? GetHighlightColor(BindableObject bindable) => (Color?)bindable.GetValue(HighlightColorProperty);

    public static void SetHighlightColor(BindableObject bindable, Color? value) => bindable.SetValue(HighlightColorProperty, value);

    private static void OnHighlightTriggerChanged(BindableObject bindable, object? oldValue, object? newValue)
    {
        // 初回バインドではハイライトしない
        if ((bindable is not VisualElement element) || (oldValue is null) || (newValue is null))
        {
            return;
        }

        var highlight = GetHighlightColor(bindable);
        if (highlight is null)
        {
            return;
        }

        var original = element.BackgroundColor ?? Colors.Transparent;

        element.AbortAnimation(HighlightAnimationName);
        element.Animate(
            HighlightAnimationName,
            v => element.BackgroundColor = LerpColor(highlight, original, v),
            16,
            600,
            Easing.CubicOut,
            (_, _) => element.BackgroundColor = original);
    }

    private static Color LerpColor(Color from, Color to, double t) =>
        new(
            (float)(from.Red + ((to.Red - from.Red) * t)),
            (float)(from.Green + ((to.Green - from.Green) * t)),
            (float)(from.Blue + ((to.Blue - from.Blue) * t)),
            (float)(from.Alpha + ((to.Alpha - from.Alpha) * t)));
}
