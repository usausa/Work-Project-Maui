namespace Template.MobileApp.Controls;

using Microsoft.Maui.Controls.Shapes;

// 下から出るシート (自作)。ページの最前面に置く。IsOpen で半開の状態から表示し、
// シートのドラッグで 半開 ⇔ 全開 ⇔ 閉じる、背景のタップで閉じる
public sealed class BottomSheetView : Grid
{
    private const string AnimationName = "SheetMove";

    private const uint Duration = 250;

    private const double BackdropOpacity = 0.4;

    public static readonly BindableProperty IsOpenProperty = BindableProperty.Create(
        nameof(IsOpen),
        typeof(bool),
        typeof(BottomSheetView),
        false,
        BindingMode.TwoWay,
        propertyChanged: static (bindable, _, newValue) => ((BottomSheetView)bindable).OnIsOpenChanged((bool)newValue));

    public static readonly BindableProperty SheetContentProperty = BindableProperty.Create(
        nameof(SheetContent),
        typeof(View),
        typeof(BottomSheetView),
        propertyChanged: static (bindable, _, newValue) => ((BottomSheetView)bindable).contentHost.Content = (View?)newValue);

    // 半開のときにシートが占める高さの割合
    public static readonly BindableProperty HalfExpandedRatioProperty = BindableProperty.Create(
        nameof(HalfExpandedRatio),
        typeof(double),
        typeof(BottomSheetView),
        0.5d);

    // 全開のときにシートが占める高さの割合
    public static readonly BindableProperty ExpandedRatioProperty = BindableProperty.Create(
        nameof(ExpandedRatio),
        typeof(double),
        typeof(BottomSheetView),
        0.92d,
        propertyChanged: static (bindable, _, _) => ((BottomSheetView)bindable).UpdateSheetSize());

    public static readonly BindableProperty CornerRadiusProperty = BindableProperty.Create(
        nameof(CornerRadius),
        typeof(double),
        typeof(BottomSheetView),
        16d,
        propertyChanged: static (bindable, _, newValue) => ((BottomSheetView)bindable).sheet.StrokeShape = CreateShape((double)newValue));

    public static readonly BindableProperty SheetBackgroundColorProperty = BindableProperty.Create(
        nameof(SheetBackgroundColor),
        typeof(Color),
        typeof(BottomSheetView),
        Colors.White,
        propertyChanged: static (bindable, _, newValue) => ((BottomSheetView)bindable).sheet.BackgroundColor = (Color)newValue);

    private readonly BoxView backdrop;

    private readonly Border sheet;

    private readonly ContentView contentHost;

    private double panStart;

    private bool expanded;

    // 表示前 (高さ未確定) に開かれたときは、レイアウト後に開く
    private bool pendingOpen;

    public bool IsOpen
    {
        get => (bool)GetValue(IsOpenProperty);
        set => SetValue(IsOpenProperty, value);
    }

    public View? SheetContent
    {
        get => (View?)GetValue(SheetContentProperty);
        set => SetValue(SheetContentProperty, value);
    }

    public double HalfExpandedRatio
    {
        get => (double)GetValue(HalfExpandedRatioProperty);
        set => SetValue(HalfExpandedRatioProperty, value);
    }

    public double ExpandedRatio
    {
        get => (double)GetValue(ExpandedRatioProperty);
        set => SetValue(ExpandedRatioProperty, value);
    }

    public double CornerRadius
    {
        get => (double)GetValue(CornerRadiusProperty);
        set => SetValue(CornerRadiusProperty, value);
    }

    public Color SheetBackgroundColor
    {
        get => (Color)GetValue(SheetBackgroundColorProperty);
        set => SetValue(SheetBackgroundColorProperty, value);
    }

    private double SheetHeight => Height * ExpandedRatio;

    private double HalfY => Math.Max(0, SheetHeight - (Height * HalfExpandedRatio));

    public BottomSheetView()
    {
        IsVisible = false;

        backdrop = new BoxView { Color = Colors.Black, Opacity = 0 };
        var tap = new TapGestureRecognizer();
        tap.Tapped += (_, _) => IsOpen = false;
        backdrop.GestureRecognizers.Add(tap);

        var grabber = new BoxView
        {
            WidthRequest = 40,
            HeightRequest = 4,
            CornerRadius = 2,
            Color = Color.FromArgb("#BDBDBD"),
            HorizontalOptions = LayoutOptions.Center,
            Margin = new Thickness(0, 10, 0, 6)
        };
        contentHost = new ContentView();
        // ジェスチャは Border ではなくレイアウト (Grid) に付ける
        var body = new Grid
        {
            RowDefinitions = { new RowDefinition(GridLength.Auto), new RowDefinition(GridLength.Star) },
            BackgroundColor = Colors.Transparent
        };
        body.Add(grabber);
        body.Add(contentHost, 0, 1);
        var pan = new PanGestureRecognizer();
        pan.PanUpdated += OnPanUpdated;
        body.GestureRecognizers.Add(pan);

        sheet = new Border
        {
            VerticalOptions = LayoutOptions.End,
            BackgroundColor = SheetBackgroundColor,
            StrokeThickness = 0,
            StrokeShape = CreateShape(CornerRadius),
            Padding = 0,
            Content = body
        };

        Children.Add(backdrop);
        Children.Add(sheet);

        SizeChanged += (_, _) => UpdateSheetSize();
    }

    private static RoundRectangle CreateShape(double radius) => new() { CornerRadius = new CornerRadius(radius, radius, 0, 0) };

    private void UpdateSheetSize()
    {
        if (Height <= 0)
        {
            return;
        }

        sheet.HeightRequest = SheetHeight;
        if (pendingOpen)
        {
            pendingOpen = false;
            sheet.TranslationY = SheetHeight;
            MoveTo(HalfY);
        }
        else if (!sheet.AnimationIsRunning(AnimationName))
        {
            sheet.TranslationY = IsOpen ? (expanded ? 0 : HalfY) : SheetHeight;
        }
    }

    private void OnIsOpenChanged(bool open)
    {
        if (open)
        {
            expanded = false;
            IsVisible = true;
            if (Height > 0)
            {
                sheet.HeightRequest = SheetHeight;
                sheet.TranslationY = SheetHeight;
                MoveTo(HalfY);
            }
            else
            {
                pendingOpen = true;
            }
        }
        else
        {
            pendingOpen = false;
            sheet.AbortAnimation(AnimationName);
            var start = sheet.TranslationY;
            var end = SheetHeight;
            var startOpacity = backdrop.Opacity;
            sheet.Animate(
                AnimationName,
                v =>
                {
                    sheet.TranslationY = start + ((end - start) * v);
                    backdrop.Opacity = startOpacity * (1 - v);
                },
                16,
                Duration,
                Easing.CubicIn,
                (_, _) => IsVisible = IsOpen);
        }
    }

    private void MoveTo(double y)
    {
        sheet.AbortAnimation(AnimationName);
        var start = sheet.TranslationY;
        var startOpacity = backdrop.Opacity;
        var endOpacity = BackdropOpacity * (1 - (y / SheetHeight));
        sheet.Animate(
            AnimationName,
            v =>
            {
                sheet.TranslationY = start + ((y - start) * v);
                backdrop.Opacity = startOpacity + ((endOpacity - startOpacity) * v);
            },
            16,
            Duration,
            Easing.CubicOut);
    }

    private void OnPanUpdated(object? sender, PanUpdatedEventArgs e)
    {
        switch (e.StatusType)
        {
            case GestureStatus.Started:
                sheet.AbortAnimation(AnimationName);
                panStart = sheet.TranslationY;
                break;
            case GestureStatus.Running:
                var y = Math.Clamp(panStart + e.TotalY, 0, SheetHeight);
                sheet.TranslationY = y;
                backdrop.Opacity = BackdropOpacity * (1 - (y / SheetHeight));
                break;
            case GestureStatus.Completed:
            case GestureStatus.Canceled:
                Settle();
                break;
        }
    }

    // 離した位置から最も近い状態へ (半開と閉じるの中間より下なら閉じる)
    private void Settle()
    {
        var y = sheet.TranslationY;
        if (y > (HalfY + SheetHeight) / 2)
        {
            IsOpen = false;
            return;
        }

        expanded = y < HalfY / 2;
        MoveTo(expanded ? 0 : HalfY);
    }
}
