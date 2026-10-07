namespace Template.MobileApp.Controls;

// アイコン+テキストの小型状態チップ。状態に応じて ChipColor / TextColor を差し替えて使う
public partial class StatusChip
{
    public static readonly BindableProperty TextProperty = BindableProperty.Create(
        nameof(Text),
        typeof(string),
        typeof(StatusChip),
        string.Empty);

    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public static readonly BindableProperty IconProperty = BindableProperty.Create(
        nameof(Icon),
        typeof(string),
        typeof(StatusChip),
        string.Empty);

    public string Icon
    {
        get => (string)GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    // 既定値は GrayLighten4 相当
    public static readonly BindableProperty ChipColorProperty = BindableProperty.Create(
        nameof(ChipColor),
        typeof(Color),
        typeof(StatusChip),
        Color.FromArgb("#F5F5F5"));

    public Color ChipColor
    {
        get => (Color)GetValue(ChipColorProperty);
        set => SetValue(ChipColorProperty, value);
    }

    // 既定値は BlueGrayDarken1 相当
    public static readonly BindableProperty IconColorProperty = BindableProperty.Create(
        nameof(IconColor),
        typeof(Color),
        typeof(StatusChip),
        Color.FromArgb("#546E7A"));

    public Color IconColor
    {
        get => (Color)GetValue(IconColorProperty);
        set => SetValue(IconColorProperty, value);
    }

    // 既定値は BlueGrayDarken2 相当
    public static readonly BindableProperty TextColorProperty = BindableProperty.Create(
        nameof(TextColor),
        typeof(Color),
        typeof(StatusChip),
        Color.FromArgb("#455A64"));

    public Color TextColor
    {
        get => (Color)GetValue(TextColorProperty);
        set => SetValue(TextColorProperty, value);
    }

    // 指定すると ChipColor / IconColor / TextColor をまとめて決める
    public static readonly BindableProperty ToneProperty = BindableProperty.Create(
        nameof(Tone),
        typeof(StatusTone),
        typeof(StatusChip),
        StatusTone.Neutral,
        propertyChanged: OnToneChanged);

    public StatusTone Tone
    {
        get => (StatusTone)GetValue(ToneProperty);
        set => SetValue(ToneProperty, value);
    }

    public StatusChip()
    {
        InitializeComponent();
    }

    private static void OnToneChanged(BindableObject bindable, object? oldValue, object? newValue)
    {
        var chip = (StatusChip)bindable;
        // 地は Lighten4、文字とアイコンは Darken2 相当(琥珀は Darken3)。Neutral は既定値
        var (chipColor, iconColor, textColor) = (StatusTone)newValue! switch
        {
            StatusTone.Success => (Color.FromArgb("#C8E6C9"), Color.FromArgb("#388E3C"), Color.FromArgb("#388E3C")),
            StatusTone.Warning => (Color.FromArgb("#FFECB3"), Color.FromArgb("#FF8F00"), Color.FromArgb("#FF8F00")),
            StatusTone.Error => (Color.FromArgb("#FFCDD2"), Color.FromArgb("#D32F2F"), Color.FromArgb("#D32F2F")),
            _ => (Color.FromArgb("#F5F5F5"), Color.FromArgb("#546E7A"), Color.FromArgb("#455A64"))
        };
        chip.ChipColor = chipColor;
        chip.IconColor = iconColor;
        chip.TextColor = textColor;
    }
}
