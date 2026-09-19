namespace Template.MobileApp.Models.Api;

public sealed class DataUpdateRequest
{
    public string Name { get; set; } = default!;

    public int Value { get; set; }
}
