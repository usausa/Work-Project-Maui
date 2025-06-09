namespace Template.MobileApp.Components.Nfc;

#pragma warning disable CA1819
public interface INfc
{
    byte[] Id { get; }

    byte[] Access(byte[] command);
}
#pragma warning restore CA1819

public interface INfcReader
{
    public NfcType NfcType { get; set; }

    public bool Enable { get; set; }

    IObservable<INfc> Discovered { get; }
}
