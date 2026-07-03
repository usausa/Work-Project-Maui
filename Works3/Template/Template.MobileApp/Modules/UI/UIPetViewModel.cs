namespace Template.MobileApp.Modules.UI;

public sealed class UIPetTag
{
    public string Label { get; init; } = string.Empty;
    public Color BackgroundColor { get; init; } = Colors.LightGray;
    public Color TextColor { get; init; } = Colors.DarkGray;
}

public sealed class UIPetViewModel : AppViewModelBase
{
    public string PetName { get; } = "Milo";

    public string Breed { get; } = "Shiba Inu";

    public string Level { get; } = "Lv.12";

    public string FlavorText { get; } =
        "A spirited companion from the eastern hills. Quick on its feet, loyal to a fault — never misses a squirrel, never forgets a friend.";

    public IReadOnlyList<UIPetTag> Tags { get; } =
    [
        new() { Label = "Playful", BackgroundColor = Color.FromArgb("#FCE4EC"), TextColor = Color.FromArgb("#C2185B") },
        new() { Label = "Friendly", BackgroundColor = Color.FromArgb("#E8F5E9"), TextColor = Color.FromArgb("#2E7D32") },
        new() { Label = "Curious", BackgroundColor = Color.FromArgb("#E3F2FD"), TextColor = Color.FromArgb("#1565C0") }
    ];

    protected override Task OnNotifyBackAsync() => Navigator.ForwardAsync(ViewId.UIMenu);

    protected override Task OnNotifyFunction1() => OnNotifyBackAsync();
}
