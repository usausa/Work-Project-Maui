namespace Baseline.FormsApp.Components.Nfc
{
    public interface INfcTag
    {
        byte[] Access(byte[] command);
    }
}
