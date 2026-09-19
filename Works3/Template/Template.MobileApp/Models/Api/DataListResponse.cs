namespace Template.MobileApp.Models.Api;

public sealed class DataListResponseEntry
{
    public long Id { get; set; }

    public string Name { get; set; } = default!;
}

#pragma warning disable CA1819
public sealed class DataListResponse
{
    public DataListResponseEntry[] Entries { get; set; } = default!;

    // 全件数 (範囲指定時の追加読み込みの終端判定)
    public int Total { get; set; }
}
#pragma warning restore CA1819
