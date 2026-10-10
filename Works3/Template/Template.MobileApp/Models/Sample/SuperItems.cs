namespace Template.MobileApp.Models.Sample;

public sealed class SuperBanner
{
    public required string Image { get; init; }

    public required string Title { get; init; }

    public required string Sub { get; init; }
}

public sealed class SuperApp
{
    public required string Glyph { get; init; }

    public required string Name { get; init; }

    public required Color Color { get; init; }
}

public sealed partial class SuperCoupon : ObservableObject
{
    public required string Title { get; init; }

    public required string Sub { get; init; }

    public required Brush Background { get; init; }

    [ObservableProperty]
    public partial bool IsAcquired { get; set; }
}

public enum SuperCrowd
{
    Low,
    Medium,
    High
}

public sealed class SuperShop
{
    public required string Glyph { get; init; }

    public required string Name { get; init; }

    public required string Category { get; init; }

    public required Color Color { get; init; }

    public required int Distance { get; init; }

    public required SuperCrowd Crowd { get; init; }
}
