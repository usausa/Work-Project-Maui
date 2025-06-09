namespace Template.MobileApp.Components.Nfc;

#pragma warning disable CA1819
public sealed class ReadBlock
{
    public byte BlockNo { get; set; }

    public byte[] BlockData { get; set; } = default!;
}
#pragma warning restore CA1819
