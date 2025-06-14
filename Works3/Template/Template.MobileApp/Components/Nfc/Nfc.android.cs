namespace Template.MobileApp.Components.Nfc;

using Android.Nfc;
using Android.Nfc.Tech;

#pragma warning disable CA1819
public sealed class AndroidNfcF : INfc
{
    private readonly NfcF nfc;

    public AndroidNfcF(NfcF nfc)
    {
        this.nfc = nfc;
    }

    public byte[] Access(byte[] command)
    {
        try
        {
            var response = nfc.Transceive(command);
            return response ?? [];
        }
        catch (TagLostException)
        {
            return [];
        }
    }
}
#pragma warning restore CA1819
