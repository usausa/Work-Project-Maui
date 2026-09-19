namespace Template.MobileApp.Models.Api;

public sealed class DataCreateRequest
{
    public string Name { get; set; } = default!;

    public int Value { get; set; }
}
