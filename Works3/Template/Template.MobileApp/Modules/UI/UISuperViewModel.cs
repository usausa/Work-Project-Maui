namespace Template.MobileApp.Modules.UI;

public sealed partial class UISuperViewModel : AppViewModelBase
{
    private readonly IDispatcherTimer timer;

    [ObservableProperty]
    public partial int BannerPosition { get; set; }

    public int Point { get; } = 1250;

    public IReadOnlyList<SuperBanner> Banners { get; } =
    [
        new() { Image = "banner01.jpg", Title = "オータムセール開催中", Sub = "全館ポイント 5 倍キャンペーン" },
        new() { Image = "banner02.jpg", Title = "新作ギフト入荷", Sub = "ラッピング無料でお届け" },
        new() { Image = "banner03.jpg", Title = "ポイントキャンペーン", Sub = "期間限定ポイント 2 倍" }
    ];

    public IReadOnlyList<SuperApp> Apps { get; } =
    [
        new() { Glyph = Fonts.MaterialIcons.Payments, Name = "支払い", Color = Color.FromArgb("#1E88E5") },
        new() { Glyph = Fonts.MaterialIcons.Receipt_long, Name = "家計簿", Color = Color.FromArgb("#43A047") },
        new() { Glyph = Fonts.MaterialIcons.Local_shipping, Name = "配送", Color = Color.FromArgb("#FB8C00") },
        new() { Glyph = Fonts.MaterialIcons.Confirmation_number, Name = "チケット", Color = Color.FromArgb("#8E24AA") },
        new() { Glyph = Fonts.MaterialIcons.Restaurant, Name = "フード", Color = Color.FromArgb("#E53935") },
        new() { Glyph = Fonts.MaterialIcons.Directions_car, Name = "タクシー", Color = Color.FromArgb("#00ACC1") },
        new() { Glyph = Fonts.MaterialIcons.Hotel, Name = "ホテル", Color = Color.FromArgb("#5E35B1") },
        new() { Glyph = Fonts.MaterialIcons.More_horiz, Name = "その他", Color = Color.FromArgb("#757575") }
    ];

    public IReadOnlyList<SuperCoupon> Coupons { get; } =
    [
        new()
        {
            Title = "コーヒー 100 円引き",
            Sub = "対象店舗限定 / 7月末まで",
            Background = new LinearGradientBrush(
                [new GradientStop(Color.FromArgb("#FF8A65"), 0f), new GradientStop(Color.FromArgb("#FF5252"), 1f)],
                new Point(0, 0),
                new Point(1, 1))
        },
        new()
        {
            Title = "送料無料クーポン",
            Sub = "3,000 円以上の注文で利用可",
            Background = new LinearGradientBrush(
                [new GradientStop(Color.FromArgb("#4FC3F7"), 0f), new GradientStop(Color.FromArgb("#1E88E5"), 1f)],
                new Point(0, 0),
                new Point(1, 1))
        },
        new()
        {
            Title = "ポイント 2 倍デー",
            Sub = "毎週金曜日はポイントアップ",
            Background = new LinearGradientBrush(
                [new GradientStop(Color.FromArgb("#81C784"), 0f), new GradientStop(Color.FromArgb("#2E7D32"), 1f)],
                new Point(0, 0),
                new Point(1, 1))
        },
        new()
        {
            Title = "映画 300 円引き",
            Sub = "レイトショー限定",
            Background = new LinearGradientBrush(
                [new GradientStop(Color.FromArgb("#BA68C8"), 0f), new GradientStop(Color.FromArgb("#6A1B9A"), 1f)],
                new Point(0, 0),
                new Point(1, 1))
        }
    ];

    public IReadOnlyList<SuperShop> Shops { get; } =
    [
        new() { Glyph = Fonts.MaterialIcons.Local_cafe, Name = "カフェ モカ", Category = "カフェ", Color = Color.FromArgb("#6D4C41"), Distance = 120, Crowd = SuperCrowd.Low },
        new() { Glyph = Fonts.MaterialIcons.Ramen_dining, Name = "麺処 ひだまり", Category = "ラーメン", Color = Color.FromArgb("#E53935"), Distance = 350, Crowd = SuperCrowd.High },
        new() { Glyph = Fonts.MaterialIcons.Local_grocery_store, Name = "フードマート", Category = "スーパー", Color = Color.FromArgb("#43A047"), Distance = 480, Crowd = SuperCrowd.Medium },
        new() { Glyph = Fonts.MaterialIcons.Bakery_dining, Name = "パン工房 ルナ", Category = "パン", Color = Color.FromArgb("#FB8C00"), Distance = 620, Crowd = SuperCrowd.Low },
        new() { Glyph = Fonts.MaterialIcons.Local_pharmacy, Name = "あおば薬局", Category = "ドラッグストア", Color = Color.FromArgb("#1E88E5"), Distance = 780, Crowd = SuperCrowd.Medium },
        new() { Glyph = Fonts.MaterialIcons.Menu_book, Name = "ブックス 青空", Category = "書店", Color = Color.FromArgb("#5E35B1"), Distance = 930, Crowd = SuperCrowd.Low }
    ];

    public IObserveCommand AcquireCommand { get; }

    //--------------------------------------------------------------------------------
    // Constructor
    //--------------------------------------------------------------------------------

    public UISuperViewModel(IDispatcher dispatcher)
    {
        // バナーは 5 秒毎に自動スライドする
        timer = dispatcher.CreateTimer();
        timer.Interval = TimeSpan.FromSeconds(5);
        Disposables.Add(timer.TickAsObservable().Subscribe(_ => BannerPosition = (BannerPosition + 1) % Banners.Count));

        AcquireCommand = MakeDelegateCommand<SuperCoupon>(x => x.IsAcquired = !x.IsAcquired);
    }

    //--------------------------------------------------------------------------------
    // Navigation
    //--------------------------------------------------------------------------------

    public override Task OnNavigatedToAsync(INavigationContext context)
    {
        timer.Start();
        return Task.CompletedTask;
    }

    public override Task OnNavigatingFromAsync(INavigationContext context)
    {
        timer.Stop();
        return Task.CompletedTask;
    }

    protected override Task OnNotifyBackAsync() => Navigator.ForwardAsync(ViewId.UIMenu1);

    protected override Task OnNotifyFunction1() => OnNotifyBackAsync();
}
