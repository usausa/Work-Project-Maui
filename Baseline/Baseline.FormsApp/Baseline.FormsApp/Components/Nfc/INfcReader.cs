namespace Baseline.FormsApp.Components.Nfc
{
    using System;

    public interface INfcReader
    {
        event EventHandler<TagDetectEventArgs> Detected;

        bool Open();

        void Close();
    }
}
