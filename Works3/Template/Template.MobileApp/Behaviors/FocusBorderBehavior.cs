namespace Template.MobileApp.Behaviors;

using Smart.Maui.Interactivity;

// 同一名前空間の static class Border(attached property)と区別する
using MauiBorder = Microsoft.Maui.Controls.Border;

// 入力コントロールのフォーカスに合わせて親 Border の枠線を色・太さアニメーション付きで強調する
public sealed class FocusBorderBehavior : BehaviorBase<VisualElement>
{
    private const string AnimationName = "FocusBorderAnimation";

    public static readonly BindableProperty FocusedStrokeProperty = BindableProperty.Create(
        nameof(FocusedStroke),
        typeof(Color),
        typeof(FocusBorderBehavior),
        Colors.Blue);

    public Color FocusedStroke
    {
        get => (Color)GetValue(FocusedStrokeProperty);
        set => SetValue(FocusedStrokeProperty, value);
    }

    public static readonly BindableProperty FocusedThicknessProperty = BindableProperty.Create(
        nameof(FocusedThickness),
        typeof(double),
        typeof(FocusBorderBehavior),
        2d);

    public double FocusedThickness
    {
        get => (double)GetValue(FocusedThicknessProperty);
        set => SetValue(FocusedThicknessProperty, value);
    }

    // 通常時の枠は初回フォーカス時に親 Border から取り込む
    private Color normalStroke = Colors.Transparent;

    private double normalThickness;

    private bool captured;

    protected override void OnAttachedTo(VisualElement bindable)
    {
        base.OnAttachedTo(bindable);

        bindable.Focused += OnFocused;
        bindable.Unfocused += OnUnfocused;
    }

    protected override void OnDetachingFrom(VisualElement bindable)
    {
        bindable.Focused -= OnFocused;
        bindable.Unfocused -= OnUnfocused;

        base.OnDetachingFrom(bindable);
    }

    private void OnFocused(object? sender, FocusEventArgs e) => AnimateBorder(focused: true);

    private void OnUnfocused(object? sender, FocusEventArgs e) => AnimateBorder(focused: false);

    private void AnimateBorder(bool focused)
    {
        if (FindParentBorder() is not { } border)
        {
            return;
        }

        AnimateBorder(border, focused);
    }

    private void AnimateBorder(MauiBorder border, bool focused)
    {
        if (!captured)
        {
            captured = true;
            normalStroke = (border.Stroke as SolidColorBrush)?.Color ?? Colors.Transparent;
            normalThickness = border.StrokeThickness;
        }

        var fromColor = (border.Stroke as SolidColorBrush)?.Color ?? normalStroke;
        var toColor = focused ? FocusedStroke : normalStroke;
        var fromThickness = border.StrokeThickness;
        var toThickness = focused ? FocusedThickness : normalThickness;

        border.AbortAnimation(AnimationName);
        border.Animate(
            AnimationName,
            v =>
            {
                border.Stroke = new SolidColorBrush(LerpColor(fromColor, toColor, v));
                border.StrokeThickness = fromThickness + ((toThickness - fromThickness) * v);
            },
            16,
            150,
            Easing.CubicOut,
            (_, _) =>
            {
                border.Stroke = new SolidColorBrush(toColor);
                border.StrokeThickness = toThickness;
            });
    }

    private MauiBorder? FindParentBorder()
    {
        var parent = AssociatedObject?.Parent;
        while (parent is not null)
        {
            if (parent is MauiBorder border)
            {
                return border;
            }
            parent = parent.Parent;
        }
        return null;
    }

    private static Color LerpColor(Color from, Color to, double t) =>
        new(
            (float)(from.Red + ((to.Red - from.Red) * t)),
            (float)(from.Green + ((to.Green - from.Green) * t)),
            (float)(from.Blue + ((to.Blue - from.Blue) * t)),
            (float)(from.Alpha + ((to.Alpha - from.Alpha) * t)));
}
