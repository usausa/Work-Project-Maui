namespace Template.MobileApp.Modules.UI;

public sealed class UIProfileViewModel : AppViewModelBase
{
    public string Name { get; } = "山奥 うさぎ";

    public string Id { get; } = "@usausa";

    public string Bio { get; } = "山奥からこんにちは 🐰 モバイルアプリとガジェットが好きです。週末はカメラを持って出かけています。";

    public int Posts { get; } = 128;

    public int Following { get; } = 87;

    public int Followers { get; } = 1024;

    public IReadOnlyList<string> Skills { get; } = ["C#", ".NET MAUI", "SkiaSharp", "Android", "iOS", "Azure"];

    public IReadOnlyList<string> Hobbies { get; } = ["カメラ", "登山", "ゲーム", "カフェ巡り", "水彩画"];

    public IReadOnlyList<string> Photos { get; } =
    [
        "usa1_full.jpg", "usa2_full.jpg", "usa3_full.jpg", "usa4_full.jpg",
        "usa5_full.jpg", "usa6_full.jpg", "usa7_full.jpg", "usa8_full.jpg"
    ];

    protected override Task OnNotifyBackAsync() => Navigator.ForwardAsync(ViewId.UIMenu);

    protected override Task OnNotifyFunction1() => OnNotifyBackAsync();
}
