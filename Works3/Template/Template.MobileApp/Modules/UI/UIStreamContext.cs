namespace Template.MobileApp.Modules.UI;

public enum StreamShelf
{
    TopRated,
    Original,
    Trending,
    Action
}

// 作品と見る人の状態 (一覧と詳細で共有)
public sealed partial class UIStreamWork : ObservableObject
{
    public required StreamWork Work { get; init; }

    // 見た割合 (0 は未視聴)
    public double Progress { get; init; }

    public bool HasProgress => Progress > 0;

    public int RemainMinutes => (int)Math.Ceiling(Work.Minutes * (1 - Progress));

    [ObservableProperty]
    public partial bool InMyList { get; set; }

    [ObservableProperty]
    public partial bool IsFavorite { get; set; }

    [ObservableProperty]
    public partial bool IsDownloaded { get; set; }
}

public sealed class UIStreamSection
{
    public required StreamShelf Shelf { get; init; }

    public required IReadOnlyList<UIStreamWork> Items { get; init; }

    public int Delay { get; init; }
}

// 一覧と詳細が共有する (一覧を開いている間)
public sealed partial class UIStreamContext : ObservableObject
{
    private const int RelatedCount = 4;

    public IReadOnlyList<UIStreamWork> Works { get; } =
        StreamCatalog.Works.Select(static x => new UIStreamWork { Work = x, Progress = SampleProgress(x.Id) }).ToArray();

    // 詳細の画面の作品 (Push の前に設定する。関連の作品を選ぶと入れ替わる)
    [ObservableProperty]
    public partial UIStreamWork Selected { get; set; } = default!;

    [ObservableProperty]
    public partial IReadOnlyList<UIStreamWork> Related { get; set; } = [];

    // 関連は同じジャンルの作品を先に
    public void Open(UIStreamWork work)
    {
        Selected = work;
        Related = Works
            .Where(x => x != work)
            .OrderBy(x => x.Work.Genre == work.Work.Genre ? 0 : 1)
            .Take(RelatedCount)
            .ToArray();
    }

    // 見本の視聴の進み具合
    private static double SampleProgress(int id) => id switch
    {
        1 => 0.4,
        5 => 0.7,
        6 => 0.25,
        _ => 0
    };
}
