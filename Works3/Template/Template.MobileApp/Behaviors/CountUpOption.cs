namespace Template.MobileApp.Behaviors;

// Label に設定した数値をカウントアップアニメーションしながら表示する
public static class CountUpOption
{
    private const string CountUpAnimationName = "CountUpOptionAnimation";

    private const uint DefaultDuration = 800;

    public static readonly BindableProperty DurationProperty = BindableProperty.CreateAttached(
        "Duration",
        typeof(int),
        typeof(CountUpOption),
        (int)DefaultDuration);

    public static int GetDuration(BindableObject bindable) => (int)bindable.GetValue(DurationProperty);

    public static void SetDuration(BindableObject bindable, int value) => bindable.SetValue(DurationProperty, value);

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

        if (label.IsLoaded)
        {
            StartCountUp(label, (double)newValue);
        }
        else
        {
            // 表示前に値が確定した場合は Loaded 時に 0 から数え上げる
            label.Loaded -= OnLabelLoaded;
            label.Loaded += OnLabelLoaded;
        }
    }

    private static void OnLabelLoaded(object? sender, EventArgs e)
    {
        if (sender is Label label)
        {
            label.Loaded -= OnLabelLoaded;
            StartCountUp(label, GetValue(label));
        }
    }

    private static void StartCountUp(Label label, double to)
    {
        var from = (double)label.GetValue(DisplayValueProperty);
        var format = GetFormat(label);

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
            (uint)GetDuration(label),
            Easing.CubicOut,
            (_, _) =>
            {
                label.SetValue(DisplayValueProperty, to);
                label.Text = string.Format(CultureInfo.CurrentCulture, format, to);
            });
    }
}
