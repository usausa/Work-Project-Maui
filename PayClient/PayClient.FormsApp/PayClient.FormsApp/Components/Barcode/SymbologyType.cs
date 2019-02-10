namespace PayClient.FormsApp.Components.Barcode
{
    using System;

    [Flags]
    public enum SymbologyType
    {
        None = 0,
        QR = 1 << 0
    }
}
