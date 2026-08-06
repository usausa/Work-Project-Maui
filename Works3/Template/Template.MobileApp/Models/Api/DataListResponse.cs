namespace Template.MobileApp.Models.Api;

public sealed class DataListResponseEntry
{
    public int Id { get; set; }

    public string Name { get; set; } = default!;
}

// バインディング/プロトコル上の都合で配列プロパティを許容するため抑止
#pragma warning disable CA1819
public sealed class DataListResponse
{
    public DataListResponseEntry[] Entries { get; set; } = default!;
}
#pragma warning restore CA1819
