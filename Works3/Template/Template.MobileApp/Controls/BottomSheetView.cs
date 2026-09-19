namespace Template.MobileApp.Controls;

using Microsoft.Maui.Controls.Shapes;

// 下から出るシート (自作)。ページの最前面に置く。IsOpen で半開の状態から表示し、
// シートのドラッグで 半開 ⇔ 全開 ⇔ 閉じる、背景のタップで閉じる
// ドラッグの終了時は、動かした距離か速さがしきい値を超えていればその方向の状態へ、そうでなければ離した位置に近い状態へ
public sealed class BottomSheetView : Grid
{
    private const string AnimationName = "SheetMove";

    private const uint Duration = 250;

    private const double BackdropOpacity = 0.4;

    // 状態の移動を確定する移動量 (dp) と速さ (dp/ms)。短い操作でも方向が明確なら従う
    private const double SwipeDistance = 16;

    private const double SwipeVelocity = 0.2;

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

    private double panTotal;

    private double panVelocity;

    private long panTimestamp;

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
                panTotal = 0;
                panVelocity = 0;
                panTimestamp = Environment.TickCount64;
                break;
            case GestureStatus.Running:
                var now = Environment.TickCount64;
                var elapsed = now - panTimestamp;
                if (elapsed > 0)
                {
                    // 直前の区間の速さ (揺れを抑えるため前回と平均する)
                    var velocity = (e.TotalY - panTotal) / elapsed;
                    panVelocity = panVelocity == 0 ? velocity : (panVelocity + velocity) / 2;
                    panTimestamp = now;
                }

                panTotal = e.TotalY;
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

    // 下方向のスワイプは 全開 → 半開 → 閉じる、上方向は 全開 へ。方向が明確でなければ離した位置から最も近い状態へ
    private void Settle()
    {
        var y = sheet.TranslationY;
        if ((panTotal >= SwipeDistance) || (panVelocity >= SwipeVelocity))
        {
            if (expanded && (y < HalfY))
            {
                expanded = false;
                MoveTo(HalfY);
            }
            else
            {
                IsOpen = false;
            }

            return;
        }

        if ((panTotal <= -SwipeDistance) || (panVelocity <= -SwipeVelocity))
        {
            expanded = true;
            MoveTo(0);
            return;
        }

        if (y > (HalfY + SheetHeight) / 2)
        {
            IsOpen = false;
            return;
        }

        expanded = y < HalfY / 2;
        MoveTo(expanded ? 0 : HalfY);
    }
}
