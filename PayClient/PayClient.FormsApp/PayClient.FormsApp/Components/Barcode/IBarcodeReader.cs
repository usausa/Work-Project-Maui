namespace PayClient.FormsApp.Components.Barcode
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IBarcodeReader
    {
        void SetSymbology(SymbologyType symbology);

        Task<ScanResult> Scan();
    }
}
