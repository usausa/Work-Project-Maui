namespace Baseline.FormsApp.Droid.Components.Nfc
{
    using System;

    using Android.Nfc;
    using Android.Nfc.Tech;

    using Baseline.FormsApp.Components.Nfc;

    public sealed class NfcTag : INfcTag
    {
        private readonly NfcF nfc;

        public NfcTag(NfcF nfc)
        {
            this.nfc = nfc;
        }

        public byte[] Access(byte[] command)
        {
            System.Diagnostics.Debug.WriteLine("SEND: " + BitConverter.ToString(command));
            try
            {
                var response = nfc.Transceive(command);
                System.Diagnostics.Debug.WriteLine("RECV: " + BitConverter.ToString(response));
                return response;
            }
            catch (TagLostException)
            {
                System.Diagnostics.Debug.WriteLine("RECV: ");
                return Array.Empty<byte>();
            }
        }
    }
}