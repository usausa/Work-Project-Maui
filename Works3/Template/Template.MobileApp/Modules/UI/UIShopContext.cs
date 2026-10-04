namespace Template.MobileApp.Modules.UI;

public sealed partial class UIShopProduct : ObservableObject
{
    public required ShopProduct Product { get; init; }

    [ObservableProperty]
    public partial bool IsFavorite { get; set; }
}

public sealed partial class UIShopCartItem : ObservableObject
{
    public required ShopProduct Product { get; init; }

    public required string Variant { get; init; }

    [ObservableProperty(NotifyAlso = [nameof(Amount)])]
    public partial int Quantity { get; set; }

    public int Amount => Product.Price * Quantity;
}

public sealed record UIShopOrder(string Number, DateTime DeliveryDate, int Count, int Total);

// 一覧・商品・カートが共有する (一覧を開いている間)
public sealed partial class UIShopContext : ObservableObject
{
    private const int MaxQuantity = 99;

    private const double DiscountRate = 0.1;

    private const int DeliveryDays = 2;

    private readonly TimeProvider timeProvider;

    private int orderNumber = 1290;

    public IReadOnlyList<UIShopProduct> Products { get; }

    // 商品の画面の商品 (Push の前に設定する)
    [ObservableProperty]
    public partial UIShopProduct Selected { get; set; } = default!;

    public ObservableCollection<UIShopCartItem> Cart { get; } = [];

    [ObservableProperty]
    public partial int CartCount { get; private set; }

    [ObservableProperty]
    public partial int Subtotal { get; private set; }

    [ObservableProperty]
    public partial int Discount { get; private set; }

    [ObservableProperty]
    public partial int Total { get; private set; }

    public UIShopContext(TimeProvider timeProvider)
    {
        this.timeProvider = timeProvider;

        Products = ShopCatalog.Products.Select(static x => new UIShopProduct { Product = x }).ToArray();

        // 見本のカート
        AddToCart(Products[3], "茶軸", 1);
        AddToCart(Products[4], "ホワイト", 2);
        AddToCart(Products[5], "ネイビー", 1);
    }

    // カテゴリ (null はすべて)・名前の部分一致・価格の上限 (null は上限なし) で絞り込み、並べる
    public IReadOnlyList<UIShopProduct> Find(ShopCategory? category, string text, ShopSort sort, int? maxPrice)
    {
        var query = Products.Where(x =>
            ((category is null) || (x.Product.Category == category)) &&
            (String.IsNullOrWhiteSpace(text) || x.Product.Name.Contains(text.Trim(), StringComparison.OrdinalIgnoreCase)) &&
            ((maxPrice is null) || (x.Product.Price <= maxPrice)));
        return sort switch
        {
            ShopSort.PriceLow => query.OrderBy(static x => x.Product.Price).ToArray(),
            ShopSort.PriceHigh => query.OrderByDescending(static x => x.Product.Price).ToArray(),
            ShopSort.Rating => query.OrderByDescending(static x => x.Product.Rating).ToArray(),
            _ => query.ToArray()
        };
    }

    // 同じ商品と種類の行があれば数を足す
    public void AddToCart(UIShopProduct product, string variant, int quantity)
    {
        var item = Cart.FirstOrDefault(x => (x.Product.Id == product.Product.Id) && (x.Variant == variant));
        if (item is null)
        {
            Cart.Add(new UIShopCartItem { Product = product.Product, Variant = variant, Quantity = quantity });
        }
        else
        {
            item.Quantity = Math.Min(MaxQuantity, item.Quantity + quantity);
        }

        Update();
    }

    public void Increase(UIShopCartItem item)
    {
        item.Quantity = Math.Min(MaxQuantity, item.Quantity + 1);
        Update();
    }

    // 1 より減らさない (行を消すのはスワイプ)
    public void Decrease(UIShopCartItem item)
    {
        item.Quantity = Math.Max(1, item.Quantity - 1);
        Update();
    }

    public void Remove(UIShopCartItem item)
    {
        Cart.Remove(item);
        Update();
    }

    // 注文を確定してカートを空にする
    public UIShopOrder Checkout()
    {
        var order = new UIShopOrder($"A{orderNumber++}", timeProvider.GetLocalNow().Date.AddDays(DeliveryDays), CartCount, Total);
        Cart.Clear();
        Update();
        return order;
    }

    private void Update()
    {
        CartCount = Cart.Sum(static x => x.Quantity);
        Subtotal = Cart.Sum(static x => x.Amount);
        Discount = (int)Math.Round(Subtotal * DiscountRate, MidpointRounding.AwayFromZero);
        Total = Subtotal - Discount;
    }
}
