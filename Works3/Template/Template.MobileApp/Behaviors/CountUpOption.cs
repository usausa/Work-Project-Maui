namespace Template.MobileApp.Behaviors;

// Label に設定した数値をカウントアップアニメーションしながら表示する
public static class CountUpOption
{
    private const string CountUpAnimationName = "CountUpOptionAnimation";

    private const uint Duration = 500;

    public static readonly BindableProperty ValueProperty = BindableProperty.CreateAttached(
        "Value",
        typeof(double),
        typeof(CountUpOption),
        0d,
        propertyChanged: OnValueChanged);

    public static double GetValue(BindableObject bindable) => (double)bindable.GetValue(ValueProperty);

    public static void SetValue(BindableObject bindable, double value) => bindable.SetValue(ValueProperty, value);

    public static readonly BindableProperty FormatProperty = BindableProperty.CreateAttached(
        "Format",
        typeof(string),
        typeof(CountUpOption),
        "{0:N0}");

    public static string GetFormat(BindableObject bindable) => (string)bindable.GetValue(FormatProperty);

    public static void SetFormat(BindableObject bindable, string value) => bindable.SetValue(FormatProperty, value);

    // 現在表示中の値(アニメーション中の中間値を保持し、次のアニメーションの開始点にする)
    private static readonly BindableProperty DisplayValueProperty = BindableProperty.CreateAttached(
        "DisplayValue",
        typeof(double),
        typeof(CountUpOption),
        0d);

    private static void OnValueChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is not Label label)
        {
            return;
        }

        var to = (double)newValue;
        var format = GetFormat(label);

        if (!label.IsLoaded)
        {
            // 未表示時は即時反映する
            label.SetValue(DisplayValueProperty, to);
            label.Text = string.Format(CultureInfo.CurrentCulture, format, to);
            return;
        }

        var from = (double)label.GetValue(DisplayValueProperty);

        label.AbortAnimation(CountUpAnimationName);
        label.Animate(
            CountUpAnimationName,
            v =>
            {
                var current = from + ((to - from) * v);
                label.SetValue(DisplayValueProperty, current);
                label.Text = string.Format(CultureInfo.CurrentCulture, format, current);
            },
            16,
            Duration,
            Easing.CubicOut,
            (_, _) =>
            {
                label.SetValue(DisplayValueProperty, to);
                label.Text = string.Format(CultureInfo.CurrentCulture, format, to);
            });
    }
}
