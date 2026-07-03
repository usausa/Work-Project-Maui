namespace Template.MobileApp.Modules.UI;

public sealed class UICartItem
{
    public string Title { get; init; } = string.Empty;
    public string Price { get; init; } = string.Empty;
    public string Quantity { get; init; } = string.Empty;
    public string Image { get; init; } = string.Empty;
}

public sealed class UICartViewModel : AppViewModelBase
{
    public string ItemCount { get; } = "3 items in cart";

    public IReadOnlyList<UICartItem> Items { get; } =
    [
        new() { Title = "Aqua Serum", Price = "$ 24.00", Quantity = "1", Image = "usa1_face.jpg" },
        new() { Title = "Velvet Lip", Price = "$ 18.50", Quantity = "2", Image = "usa2_face.jpg" },
        new() { Title = "Glow Cream", Price = "$ 32.00", Quantity = "1", Image = "usa3_face.jpg" }
    ];

    public string Coupon { get; } = "SPRING SALE −10%";

    public string Points { get; } = "12,540 pt available";

    public string Subtotal { get; } = "$ 93.00";

    public string Discount { get; } = "− $ 9.30";

    public string Total { get; } = "$ 83.70";

    protected override Task OnNotifyBackAsync() => Navigator.ForwardAsync(ViewId.UIItem);

    protected override Task OnNotifyFunction1() => OnNotifyBackAsync();
}
