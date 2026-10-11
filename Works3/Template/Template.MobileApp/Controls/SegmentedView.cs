namespace Template.MobileApp.Controls;

using Microsoft.Maui.Controls.Shapes;

using Template.MobileApp.Behaviors;

using Border = Microsoft.Maui.Controls.Border;

// 区切りの並びから 1 つを選ぶ。ItemsSource の項目 (文字) ごとに同じ幅の区切りを作り (区切りの間に線)、押した区切りを SelectedIndex にする。
// 選んだ区切りの塗りは選んだ区切りへ滑らせ、文字の色は塗りが重なる割合で変える
public sealed class SegmentedView : ContentView
{
    private const string AnimationName = "SegmentedViewSelection";

    private const uint Duration = 250;

    public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create(
        nameof(ItemsSource),
        typeof(IEnumerable),
        typeof(SegmentedView),
        propertyChanged: static (bindable, _, _) => ((SegmentedView)bindable).CreateSegments());

    public static readonly BindableProperty SelectedIndexProperty = BindableProperty.Create(
        nameof(SelectedIndex),
        typeof(int),
        typeof(SegmentedView),
        0,
        BindingMode.TwoWay,
        propertyChanged: static (bindable, _, newValue) => ((SegmentedView)bindable).MoveSelection((int)newValue, true));

    public static readonly BindableProperty SegmentWidthProperty = BindableProperty.Create(
        nameof(SegmentWidth),
        typeof(double),
        typeof(SegmentedView),
        100d,
        propertyChanged: static (bindable, _, _) => ((SegmentedView)bindable).CreateSegments());

    public static readonly BindableProperty SegmentHeightProperty = BindableProperty.Create(
        nameof(SegmentHeight),
        typeof(double),
        typeof(SegmentedView),
        36d,
        propertyChanged: static (bindable, _, newValue) => ((SegmentedView)bindable).grid.HeightRequest = (double)newValue);

    public static readonly BindableProperty FontSizeProperty = BindableProperty.Create(
        nameof(FontSize),
        typeof(double),
        typeof(SegmentedView),
        16d,
        propertyChanged: static (bindable, _, _) => ((SegmentedView)bindable).CreateSegments());

    public static readonly BindableProperty TextColorProperty = BindableProperty.Create(
        nameof(TextColor),
        typeof(Color),
        typeof(SegmentedView),
        Color.FromArgb("#1C1B1F"),
        propertyChanged: static (bindable, _, _) => ((SegmentedView)bindable).UpdateColors());

    public static readonly BindableProperty SelectedTextColorProperty = BindableProperty.Create(
        nameof(SelectedTextColor),
        typeof(Color),
        typeof(SegmentedView),
        Colors.White,
        propertyChanged: static (bindable, _, _) => ((SegmentedView)bindable).UpdateColors());

    public static readonly BindableProperty SelectedBackgroundProperty = BindableProperty.Create(
        nameof(SelectedBackground),
        typeof(Color),
        typeof(SegmentedView),
        Color.FromArgb("#6750A4"),
        propertyChanged: static (bindable, _, newValue) => ((SegmentedView)bindable).selection.Color = (Color)newValue);

    public static readonly BindableProperty StrokeProperty = BindableProperty.Create(
        nameof(Stroke),
        typeof(Color),
        typeof(SegmentedView),
        Color.FromArgb("#79747E"),
        propertyChanged: static (bindable, _, _) => ((SegmentedView)bindable).CreateSegments());

    public static readonly BindableProperty CornerRadiusProperty = BindableProperty.Create(
        nameof(CornerRadius),
        typeof(double),
        typeof(SegmentedView),
        20d,
        propertyChanged: static (bindable, _, newValue) => ((SegmentedView)bindable).border.StrokeShape = new RoundRectangle { CornerRadius = (double)newValue });

    private readonly Border border;

    private readonly Grid grid;

    private readonly BoxView selection;

    private readonly List<Label> labels = [];

    private readonly Command<int> selectCommand;

    // 塗りの位置 (区切りの番号。動かす間は途中の値)
    private double position;

    public IEnumerable? ItemsSource
    {
        get => (IEnumerable?)GetValue(ItemsSourceProperty);
        set => SetValue(ItemsSourceProperty, value);
    }

    public int SelectedIndex
    {
        get => (int)GetValue(SelectedIndexProperty);
        set => SetValue(SelectedIndexProperty, value);
    }

    public double SegmentWidth
    {
        get => (double)GetValue(SegmentWidthProperty);
        set => SetValue(SegmentWidthProperty, value);
    }

    public double SegmentHeight
    {
        get => (double)GetValue(SegmentHeightProperty);
        set => SetValue(SegmentHeightProperty, value);
    }

    public double FontSize
    {
        get => (double)GetValue(FontSizeProperty);
        set => SetValue(FontSizeProperty, value);
    }

    public Color TextColor
    {
        get => (Color)GetValue(TextColorProperty);
        set => SetValue(TextColorProperty, value);
    }

    public Color SelectedTextColor
    {
        get => (Color)GetValue(SelectedTextColorProperty);
        set => SetValue(SelectedTextColorProperty, value);
    }

    public Color SelectedBackground
    {
        get => (Color)GetValue(SelectedBackgroundProperty);
        set => SetValue(SelectedBackgroundProperty, value);
    }

    public Color Stroke
    {
        get => (Color)GetValue(StrokeProperty);
        set => SetValue(StrokeProperty, value);
    }

    public double CornerRadius
    {
        get => (double)GetValue(CornerRadiusProperty);
        set => SetValue(CornerRadiusProperty, value);
    }

    public SegmentedView()
    {
        HorizontalOptions = LayoutOptions.Center;
        selectCommand = new Command<int>(x => SelectedIndex = x);

        selection = new BoxView
        {
            Color = SelectedBackground,
            HorizontalOptions = LayoutOptions.Start,
            InputTransparent = true
        };
        grid = new Grid { HeightRequest = SegmentHeight };
        // 枠は中身を角で切るので、両端の区切りの塗りと波紋も丸くなる
        border = new Border
        {
            Padding = 0,
            StrokeThickness = 1,
            StrokeShape = new RoundRectangle { CornerRadius = CornerRadius },
            Content = grid
        };
        Content = border;
    }

    private void CreateSegments()
    {
        grid.Clear();
        grid.ColumnDefinitions.Clear();
        labels.Clear();
        border.Stroke = Stroke;

        var items = ItemsSource?.Cast<object?>().ToList() ?? [];
        selection.WidthRequest = SegmentWidth;
        grid.Add(selection);
        for (var i = 0; i < items.Count; i++)
        {
            if (i > 0)
            {
                grid.ColumnDefinitions.Add(new ColumnDefinition(new GridLength(1)));
                grid.Add(new BoxView { Color = Stroke, InputTransparent = true }, (i * 2) - 1);
            }

            grid.ColumnDefinitions.Add(new ColumnDefinition(new GridLength(SegmentWidth)));
        }

        for (var i = 0; i < items.Count; i++)
        {
            var label = new Label
            {
                Text = items[i]?.ToString() ?? string.Empty,
                FontSize = FontSize,
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Center
            };
            TouchOption.SetRipple(label, true);
            TouchOption.SetClickCommand(label, selectCommand);
            TouchOption.SetClickCommandParameter(label, i);
            labels.Add(label);
            grid.Add(label, i * 2);
        }

        selection.IsVisible = items.Count > 0;
        MoveSelection(SelectedIndex, false);
    }

    private void MoveSelection(int index, bool animated)
    {
        selection.AbortAnimation(AnimationName);
        if (animated && (labels.Count > 0))
        {
            var start = position;
            selection.Animate(AnimationName, v => SetPosition(start + ((index - start) * v)), 16, Duration, Easing.CubicOut);
        }
        else
        {
            SetPosition(index);
        }
    }

    private void SetPosition(double value)
    {
        position = value;
        selection.TranslationX = value * (SegmentWidth + 1);
        UpdateColors();
    }

    private void UpdateColors()
    {
        for (var i = 0; i < labels.Count; i++)
        {
            var rate = Math.Clamp(1 - Math.Abs(position - i), 0, 1);
            labels[i].TextColor = new Color(
                (float)Lerp(TextColor.Red, SelectedTextColor.Red, rate),
                (float)Lerp(TextColor.Green, SelectedTextColor.Green, rate),
                (float)Lerp(TextColor.Blue, SelectedTextColor.Blue, rate),
                (float)Lerp(TextColor.Alpha, SelectedTextColor.Alpha, rate));
        }
    }

    private static double Lerp(double from, double to, double t) => from + ((to - from) * t);
}
