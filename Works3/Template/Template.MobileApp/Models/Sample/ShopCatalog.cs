namespace Template.MobileApp.Models.Sample;

public enum ShopCategory
{
    Pc,
    Display,
    Input,
    AudioVisual,
    Storage
}

public enum ShopSort
{
    Recommended,
    PriceLow,
    PriceHigh,
    Rating
}

public sealed record ShopSpec(string Name, string Value);

// Variants が空の商品は種類を選ばない
public sealed record ShopProduct(
    int Id,
    ShopCategory Category,
    string Kind,
    string Name,
    int Price,
    string Image,
    double Rating,
    int ReviewCount,
    bool IsPopular,
    string Description,
    IReadOnlyList<ShopSpec> Specs,
    string VariantName,
    IReadOnlyList<string> Variants);

// 見本の商品 (取得の代わり)
public static class ShopCatalog
{
    public static IReadOnlyList<ShopProduct> Products { get; } =
    [
        new(
            1, ShopCategory.Pc, "ノート PC", "スリムノート PC 14", 179800, "product_device01.jpg", 4.6, 212, true,
            "14 インチの軽いノート。重さは 1.1 kg、電池は最大 18 時間。テレワークや出張の相棒に。",
            [new("CPU", "8 コア"), new("メモリ", "16 GB"), new("ストレージ", "SSD 512 GB"), new("重さ", "1.1 kg")],
            "カラー", ["シルバー", "スペースグレー"]),
        new(
            2, ShopCategory.Display, "モニター", "4K モニター 27", 64800, "product_device02.jpg", 4.5, 134, true,
            "27 インチの 4K の IPS パネル。USB-C のケーブル 1 本で映像の入力と 65 W の給電ができます。",
            [new("解像度", "3840 × 2160"), new("パネル", "IPS"), new("入力", "USB-C / HDMI × 2"), new("給電", "65 W")],
            "スタンド", ["標準", "高さ調整"]),
        new(
            3, ShopCategory.Pc, "デスクトップ", "コンパクトデスクトップ", 129800, "product_device03.jpg", 4.3, 58, true,
            "手のひらに乗る小さな本体に 12 コアの CPU。静かで、机の上を広く使えます。",
            [new("CPU", "12 コア"), new("メモリ", "16 GB"), new("ストレージ", "SSD 1 TB"), new("大きさ", "13 × 13 × 5 cm")],
            "メモリ", ["16 GB", "32 GB"]),
        new(
            4, ShopCategory.Input, "キーボード", "メカニカルキーボード", 19800, "product_gear01.jpg", 4.7, 342, true,
            "静音のメカニカルスイッチを使ったワイヤレスキーボード。有線と Bluetooth の両対応で、最大 3 台の機器をワンタッチで切り替えられます。",
            [new("接続", "USB-C / Bluetooth 5.1"), new("配列", "日本語 91 キー"), new("電池", "最大 200 時間"), new("重さ", "780 g")],
            "スイッチ", ["赤軸", "茶軸", "青軸"]),
        new(
            5, ShopCategory.Input, "マウス", "ワイヤレスマウス", 8900, "product_gear02.jpg", 4.4, 189, false,
            "手の形に沿う静音マウス。3 台の機器をボタンで切り替えられます。",
            [new("接続", "Bluetooth / USB レシーバー"), new("解像度", "最大 4000 dpi"), new("電池", "単 3 × 1(約 18 か月)"), new("重さ", "99 g")],
            "カラー", ["ホワイト", "ブラック"]),
        new(
            6, ShopCategory.AudioVisual, "ヘッドセット", "ノイズキャンセリングヘッドセット", 14800, "product_gear03.jpg", 4.5, 276, true,
            "周りの音を抑えて会議に集中。マイクは口元の声だけを拾います。",
            [new("接続", "Bluetooth 5.3 / USB-C"), new("再生", "最大 30 時間"), new("マイク", "単一指向性"), new("重さ", "250 g")],
            "カラー", ["ネイビー", "ブラック"]),
        new(
            7, ShopCategory.AudioVisual, "Web カメラ", "4K Web カメラ", 11800, "product_gear04.jpg", 4.2, 97, false,
            "4K 30 fps と HDR で、暗い部屋でも明るく映ります。レンズを隠すシャッター付き。",
            [new("解像度", "4K 30 fps"), new("画角", "90°"), new("マイク", "ステレオ"), new("接続", "USB-C")],
            string.Empty, []),
        new(
            8, ShopCategory.Storage, "SSD", "ポータブル SSD 1TB", 16800, "product_gear05.jpg", 4.6, 158, false,
            "読み込みは最大 1,050 MB/s。手のひらに収まる大きさで、落としても壊れにくい作りです。",
            [new("容量", "1 TB"), new("速度", "最大 1,050 MB/s"), new("接続", "USB 3.2 Gen 2"), new("重さ", "58 g")],
            string.Empty, []),
        new(
            9, ShopCategory.Storage, "ドック", "USB-C ドック", 27800, "product_gear06.jpg", 4.1, 73, false,
            "ケーブル 1 本で 2 画面の出力、有線 LAN、100 W の充電をまとめてつなげます。",
            [new("映像", "HDMI × 2(4K 60 Hz)"), new("USB", "4 ポート"), new("LAN", "2.5 GbE"), new("給電", "100 W")],
            string.Empty, [])
    ];
}
