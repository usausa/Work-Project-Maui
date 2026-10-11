namespace Template.MobileApp.Controls;

using Fonts;

using Template.MobileApp.Behaviors;

public enum AccordionExpandMode
{
    SingleOrNone,
    MultipleOrNone
}

// AccordionItem を縦に並べる。ExpandMode が SingleOrNone のときは、開いた項目のほかを閉じる
public sealed class AccordionView : VerticalStackLayout
{
    public static readonly BindableProperty ExpandModeProperty = BindableProperty.Create(
        nameof(ExpandMode),
        typeof(AccordionExpandMode),
        typeof(AccordionView),
        AccordionExpandMode.MultipleOrNone);

    public AccordionExpandMode ExpandMode
    {
        get => (AccordionExpandMode)GetValue(ExpandModeProperty);
        set => SetValue(ExpandModeProperty, value);
    }

    internal void OnItemExpanded(AccordionItem item)
    {
        if (ExpandMode == AccordionExpandMode.SingleOrNone)
        {
            foreach (var other in Children.OfType<AccordionItem>().Where(x => x != item))
            {
                other.IsExpanded = false;
            }
        }
    }
}

// 見出しと、見出しを押すと開閉する中身 (AccordionView に並べる)。開閉は中身の高さを伸び縮みさせ、見出しの右の矢印を回す。開いている間は見出しの上に線を引く
public sealed class AccordionItem : Grid
{
    private const string AnimationName = "AccordionItemExpand";

    private const uint Duration = 250;

    private static readonly Color IconColor = Color.FromArgb("#49454F");

    private static readonly Color LineColor = Color.FromArgb("#CAC4D0");

    public static readonly BindableProperty HeaderProperty = BindableProperty.Create(
        nameof(Header),
        typeof(View),
        typeof(AccordionItem),
        propertyChanged: static (bindable, oldValue, newValue) => ((AccordionItem)bindable).SetHeader((View?)oldValue, (View?)newValue));

    public static readonly BindableProperty ContentProperty = BindableProperty.Create(
        nameof(Content),
        typeof(View),
        typeof(AccordionItem),
        propertyChanged: static (bindable, _, newValue) => ((AccordionItem)bindable).body.Content = (View?)newValue);

    public static readonly BindableProperty IsExpandedProperty = BindableProperty.Create(
        nameof(IsExpanded),
        typeof(bool),
        typeof(AccordionItem),
        false,
        BindingMode.TwoWay,
        propertyChanged: static (bindable, _, newValue) => ((AccordionItem)bindable).OnIsExpandedChanged((bool)newValue));

    private readonly Grid headerRow;

    private readonly Label icon;

    private readonly BoxView line;

    // 中身を切り抜いて高さを動かす
    private readonly ContentView body;

    private bool revealOnExpanded;

    public View? Header
    {
        get => (View?)GetValue(HeaderProperty);
        set => SetValue(HeaderProperty, value);
    }

    public View? Content
    {
        get => (View?)GetValue(ContentProperty);
        set => SetValue(ContentProperty, value);
    }

    public bool IsExpanded
    {
        get => (bool)GetValue(IsExpandedProperty);
        set => SetValue(IsExpandedProperty, value);
    }

    public AccordionItem()
    {
        RowDefinitions.Add(new RowDefinition(GridLength.Auto));
        RowDefinitions.Add(new RowDefinition(GridLength.Auto));

        icon = new Label
        {
            Text = MaterialIcons.Expand_more,
            FontFamily = MaterialIcons.FontFamily,
            FontSize = 24,
            TextColor = IconColor,
            Margin = new Thickness(0, 0, 8, 0),
            VerticalOptions = LayoutOptions.Center,
            InputTransparent = true
        };
        headerRow = new Grid
        {
            ColumnDefinitions = [new ColumnDefinition(GridLength.Star), new ColumnDefinition(GridLength.Auto)]
        };
        Grid.SetColumn(icon, 1);
        headerRow.Children.Add(icon);
        TouchOption.SetRipple(headerRow, true);
        TouchOption.SetClickCommand(headerRow, new Command(Toggle));

        line = new BoxView
        {
            HeightRequest = 0.5,
            Color = LineColor,
            VerticalOptions = LayoutOptions.Start,
            IsVisible = false,
            InputTransparent = true
        };

        body = new ContentView { IsClippedToBounds = true, IsVisible = false };

        Add(headerRow);
        Add(line);
        this.Add(body, 0, 1);
    }

    private void SetHeader(View? oldView, View? newView)
    {
        if (oldView is not null)
        {
            headerRow.Remove(oldView);
        }

        if (newView is not null)
        {
            headerRow.Add(newView);
        }
    }

    private void Toggle()
    {
        revealOnExpanded = !IsExpanded;
        IsExpanded = !IsExpanded;
    }

    // 押して開いた項目が画面の端で隠れていれば、開き終わったときに見える位置までスクロールする
    private void Reveal()
    {
        var parent = Parent;
        while ((parent is not null) && (parent is not ScrollView))
        {
            parent = parent.Parent;
        }

        if (parent is ScrollView scroll)
        {
            _ = scroll.ScrollToAsync(this, ScrollToPosition.MakeVisible, true);
        }
    }

    private void OnIsExpandedChanged(bool expanded)
    {
        line.IsVisible = expanded;
        if (expanded && (Parent is AccordionView accordion))
        {
            accordion.OnItemExpanded(this);
        }

        this.AbortAnimation(AnimationName);
        if ((Width > 0) && (Content is IView content))
        {
            var from = body.IsVisible ? body.Height : 0;
            var to = expanded ? content.Measure(Width, Double.PositiveInfinity).Height : 0;
            var fromRotation = icon.Rotation;
            var toRotation = expanded ? 180d : 0d;
            body.HeightRequest = from;
            body.IsVisible = true;
            this.Animate(
                AnimationName,
                v =>
                {
                    body.HeightRequest = from + ((to - from) * v);
                    icon.Rotation = fromRotation + ((toRotation - fromRotation) * v);
                },
                16,
                Duration,
                Easing.Linear,
                (_, canceled) =>
                {
                    if (!canceled)
                    {
                        body.IsVisible = expanded;
                        body.HeightRequest = -1;
                        if (expanded && revealOnExpanded)
                        {
                            revealOnExpanded = false;
                            Reveal();
                        }
                    }
                });
        }
        else
        {
            // 表示の前は動かさずに決める
            body.IsVisible = expanded;
            body.HeightRequest = -1;
            icon.Rotation = expanded ? 180 : 0;
        }
    }
}
