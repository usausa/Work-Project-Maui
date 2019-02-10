namespace PayClient.FormsApp.Components.Barcode
{
    public class ScanResult
    {
        public static ScanResult None { get; } = new ScanResult(SymbologyType.None, string.Empty);

        public SymbologyType Symbology { get; }

        public string Text { get; }

        public ScanResult(SymbologyType symbology, string text)
        {
            Symbology = symbology;
            Text = text;
        }
    }
}
