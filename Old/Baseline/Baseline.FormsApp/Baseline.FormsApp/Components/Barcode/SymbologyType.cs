namespace Baseline.FormsApp.Components.Barcode
{
    using System;

    [Flags]
    public enum SymbologyType
    {
        None = 0,
        QR = 1 << 0,
        Ean13 = 1 << 1,
        Ean8 = 1 << 2
    }
}
