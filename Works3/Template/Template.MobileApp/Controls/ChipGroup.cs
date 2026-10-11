namespace Template.MobileApp.Controls;

using Microsoft.Maui.Controls.Shapes;
using Microsoft.Maui.Layouts;

using Template.MobileApp.Behaviors;

using Border = Microsoft.Maui.Controls.Border;

// 1 つだけ選ぶチップの並び。ItemsSource の項目ごとにチップ (丸い枠と文字) を作って並べ、押したチップの項目を SelectedItem にする。
// 文字は ItemTemplate の Label (無ければ項目の文字列)。文字の大きさ・色はチップの値で付ける。ChipPadding はチップの外の余白。
// Wrap で折り返す (FlexLayout)。折り返さないときは横に並べる (間は Spacing)
public sealed class ChipGroup : ContentView
{
    // 文字の左右の余白
    private const double TextPadding = 15;

    // 高さは文字の大きさの 1.3 倍に上下の余白を足す
    private const double TextHeightRate = 1.3;

    private const double HeightPadding = 13;

    public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create(
        nameof(ItemsSource),
        typeof(IEnumerable),
        typeof(ChipGroup),
        propertyChanged: static (bindable, _, _) => ((ChipGroup)bindable).CreateChips());

    public static readonly BindableProperty ItemTemplateProperty = BindableProperty.Create(
        nameof(ItemTemplate),
        typeof(DataTemplate),
        typeof(ChipGroup),
        propertyChanged: static (bindable, _, _) => ((ChipGroup)bindable).CreateChips());

    public static readonly BindableProperty SelectedItemProperty = BindableProperty.Create(
        nameof(SelectedItem),
        typeof(object),
        typeof(ChipGroup),
        null,
        BindingMode.TwoWay,
        propertyChanged: static (bindable, _, _) => ((ChipGroup)bindable).UpdateChips());

    public static readonly BindableProperty WrapProperty = BindableProperty.Create(
        nameof(Wrap),
        typeof(bool),
        typeof(ChipGroup),
        false,
        propertyChanged: static (bindable, _, _) => ((ChipGroup)bindable).CreateChips());

    public static readonly BindableProperty SpacingProperty = BindableProperty.Create(
        nameof(Spacing),
        typeof(double),
        typeof(ChipGroup),
        4d,
        propertyChanged: static (bindable, _, _) => ((ChipGroup)bindable).CreateChips());

    public static readonly BindableProperty ChipPaddingProperty = BindableProperty.Create(
        nameof(ChipPadding),
        typeof(Thickness),
        typeof(ChipGroup),
        Thickness.Zero,
        propertyChanged: static (bindable, _, _) => ((ChipGroup)bindable).CreateChips());

    public static readonly BindableProperty ChipTextSizeProperty = BindableProperty.Create(
        nameof(ChipTextSize),
        typeof(double),
        typeof(ChipGroup),
        14d,
        propertyChanged: static (bindable, _, _) => ((ChipGroup)bindable).CreateChips());

    public static readonly BindableProperty ChipCornerRadiusProperty = BindableProperty.Create(
        nameof(ChipCornerRadius),
        typeof(double),
        typeof(ChipGroup),
        16d,
        propertyChanged: static (bindable, _, _) => ((ChipGroup)bindable).CreateChips());

    public static readonly BindableProperty ChipBackgroundProperty = BindableProperty.Create(
        nameof(ChipBackground),
        typeof(Color),
        typeof(ChipGroup),
        Colors.White,
        propertyChanged: static (bindable, _, _) => ((ChipGroup)bindable).UpdateChips());

    public static readonly BindableProperty ChipStrokeProperty = BindableProperty.Create(
        nameof(ChipStroke),
        typeof(Color),
        typeof(ChipGroup),
        Colors.LightGray,
        propertyChanged: static (bindable, _, _) => ((ChipGroup)bindable).UpdateChips());

    public static readonly BindableProperty ChipTextColorProperty = BindableProperty.Create(
        nameof(ChipTextColor),
        typeof(Color),
        typeof(ChipGroup),
        Colors.Black,
        propertyChanged: static (bindable, _, _) => ((ChipGroup)bindable).UpdateChips());

    public static readonly BindableProperty SelectedChipBackgroundProperty = BindableProperty.Create(
        nameof(SelectedChipBackground),
        typeof(Color),
        typeof(ChipGroup),
        Colors.Blue,
        propertyChanged: static (bindable, _, _) => ((ChipGroup)bindable).UpdateChips());

    public static readonly BindableProperty SelectedChipTextColorProperty = BindableProperty.Create(
        nameof(SelectedChipTextColor),
        typeof(Color),
        typeof(ChipGroup),
        Colors.White,
        propertyChanged: static (bindable, _, _) => ((ChipGroup)bindable).UpdateChips());

    // 押したときの波紋の色 (既定は #1C1B1F の 12.5%)
    public static readonly BindableProperty ChipRippleColorProperty = BindableProperty.Create(
        nameof(ChipRippleColor),
        typeof(Color),
        typeof(ChipGroup),
        Color.FromArgb("#201C1B1F"),
        propertyChanged: static (bindable, _, _) => ((ChipGroup)bindable).CreateChips());

    private readonly Command<object?> selectCommand;

    public IEnumerable? ItemsSource
    {
        get => (IEnumerable?)GetValue(ItemsSourceProperty);
        set => SetValue(ItemsSourceProperty, value);
    }

    public DataTemplate? ItemTemplate
    {
        get => (DataTemplate?)GetValue(ItemTemplateProperty);
        set => SetValue(ItemTemplateProperty, value);
    }

    public object? SelectedItem
    {
        get => GetValue(SelectedItemProperty);
        set => SetValue(SelectedItemProperty, value);
    }

    public bool Wrap
    {
        get => (bool)GetValue(WrapProperty);
        set => SetValue(WrapProperty, value);
    }

    public double Spacing
    {
        get => (double)GetValue(SpacingProperty);
        set => SetValue(SpacingProperty, value);
    }

    public Thickness ChipPadding
    {
        get => (Thickness)GetValue(ChipPaddingProperty);
        set => SetValue(ChipPaddingProperty, value);
    }

    public double ChipTextSize
    {
        get => (double)GetValue(ChipTextSizeProperty);
        set => SetValue(ChipTextSizeProperty, value);
    }

    public double ChipCornerRadius
    {
        get => (double)GetValue(ChipCornerRadiusProperty);
        set => SetValue(ChipCornerRadiusProperty, value);
    }

    public Color ChipBackground
    {
        get => (Color)GetValue(ChipBackgroundProperty);
        set => SetValue(ChipBackgroundProperty, value);
    }

    public Color ChipStroke
    {
        get => (Color)GetValue(ChipStrokeProperty);
        set => SetValue(ChipStrokeProperty, value);
    }

    public Color ChipTextColor
    {
        get => (Color)GetValue(ChipTextColorProperty);
        set => SetValue(ChipTextColorProperty, value);
    }

    public Color SelectedChipBackground
    {
        get => (Color)GetValue(SelectedChipBackgroundProperty);
        set => SetValue(SelectedChipBackgroundProperty, value);
    }

    public Color SelectedChipTextColor
    {
        get => (Color)GetValue(SelectedChipTextColorProperty);
        set => SetValue(SelectedChipTextColorProperty, value);
    }

    public Color ChipRippleColor
    {
        get => (Color)GetValue(ChipRippleColorProperty);
        set => SetValue(ChipRippleColorProperty, value);
    }

    public ChipGroup()
    {
        selectCommand = new Command<object?>(x => SelectedItem = x);
    }

    private void CreateChips()
    {
        Layout layout = Wrap
            ? new FlexLayout
            {
                Wrap = FlexWrap.Wrap,
                Direction = FlexDirection.Row,
                JustifyContent = FlexJustify.Start,
                AlignItems = FlexAlignItems.Start,
                AlignContent = FlexAlignContent.Start
            }
            : new HorizontalStackLayout { Spacing = Spacing };
        if (ItemsSource is not null)
        {
            foreach (var item in ItemsSource)
            {
                layout.Children.Add(CreateChip(item));
            }
        }

        Content = layout;
        UpdateChips();
    }

    // 枠は文字を角で切るので、押したときの波紋は文字に付けて枠の形に収める
    private Border CreateChip(object? item)
    {
        var label = ItemTemplate?.CreateContent() as Label ?? new Label { Text = item?.ToString() ?? string.Empty };
        label.BindingContext = item;
        label.Padding = new Thickness(TextPadding, 0);
        label.FontSize = ChipTextSize;
        label.HorizontalTextAlignment = TextAlignment.Center;
        label.VerticalTextAlignment = TextAlignment.Center;
        TouchOption.SetRipple(label, true);
        TouchOption.SetRippleColor(label, ChipRippleColor);
        TouchOption.SetClickCommand(label, selectCommand);
        TouchOption.SetClickCommandParameter(label, item);

        return new Border
        {
            Margin = ChipPadding,
            Padding = 0,
            HeightRequest = (ChipTextSize * TextHeightRate) + HeightPadding,
            StrokeThickness = 1,
            StrokeShape = new RoundRectangle { CornerRadius = ChipCornerRadius },
            Content = label
        };
    }

    private void UpdateChips()
    {
        if (Content is Layout layout)
        {
            foreach (var chip in layout.Children.OfType<Border>())
            {
                if (chip.Content is Label label)
                {
                    var selected = Equals(label.BindingContext, SelectedItem);
                    chip.BackgroundColor = selected ? SelectedChipBackground : ChipBackground;
                    chip.Stroke = selected ? SelectedChipBackground : ChipStroke;
                    label.TextColor = selected ? SelectedChipTextColor : ChipTextColor;
                }
            }
        }
    }
}
