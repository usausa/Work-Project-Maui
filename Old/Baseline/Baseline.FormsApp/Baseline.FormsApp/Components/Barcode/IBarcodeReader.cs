namespace Baseline.FormsApp.Components.Barcode
{
    using System.Threading.Tasks;

    public interface IBarcodeReader
    {
        void SetSymbology(SymbologyType symbology);

        Task<ScanResult> Scan();
    }
}
