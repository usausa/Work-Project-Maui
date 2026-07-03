namespace Template.MobileApp.Modules.UI;

public sealed class UIShopItem
{
    public string Title { get; init; } = string.Empty;
    public string Price { get; init; } = string.Empty;
    public string Image { get; init; } = string.Empty;
}

public sealed class UIShopViewModel : AppViewModelBase
{
    public string Greeting { get; } = "Hello, Anna!";

    public string SubGreeting { get; } = "Discover your style";

    public IObserveCommand ItemCommand { get; }

    public UIShopViewModel()
    {
        ItemCommand = MakeAsyncCommand(() => Navigator.ForwardAsync(ViewId.UIItem));
    }

    public IReadOnlyList<UIShopItem> Popular { get; } =
    [
        new() { Title = "Velvet Dress", Price = "$ 89.00", Image = "usa1_full.jpg" },
        new() { Title = "Denim Jacket", Price = "$ 65.00", Image = "usa2_full.jpg" },
        new() { Title = "Summer Hat", Price = "$ 22.00", Image = "usa3_full.jpg" }
    ];

    public IReadOnlyList<UIShopItem> Items { get; } =
    [
        new() { Title = "Aqua Serum", Price = "$ 24.00", Image = "usa1_face.jpg" },
        new() { Title = "Velvet Lip", Price = "$ 18.50", Image = "usa2_face.jpg" },
        new() { Title = "Glow Cream", Price = "$ 32.00", Image = "usa3_face.jpg" },
        new() { Title = "Pure Mist", Price = "$ 12.00", Image = "usa4_face.jpg" },
        new() { Title = "Silky Mask", Price = "$ 22.00", Image = "usa5_face.jpg" },
        new() { Title = "Petal Blush", Price = "$ 15.80", Image = "usa6_face.jpg" }
    ];

    protected override Task OnNotifyBackAsync() => Navigator.ForwardAsync(ViewId.UIMenu);

    protected override Task OnNotifyFunction1() => OnNotifyBackAsync();
}
