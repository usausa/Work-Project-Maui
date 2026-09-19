namespace Template.MobileApp.Models.Api;

public sealed class StorageListResponseEntry
{
    public string Name { get; set; } = default!;

    public bool Directory { get; set; }

    // ディレクトリは null
    public long? Size { get; set; }

    public DateTime LastModified { get; set; }
}

#pragma warning disable CA1819
public sealed class StorageListResponse
{
    public StorageListResponseEntry[] Entries { get; set; } = default!;
}
#pragma warning restore CA1819
