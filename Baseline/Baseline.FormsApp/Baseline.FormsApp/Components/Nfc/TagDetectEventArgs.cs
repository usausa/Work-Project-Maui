namespace Baseline.FormsApp.Components.Nfc
{
    using System;

    public sealed class TagDetectEventArgs : EventArgs
    {
        public INfcTag Tag { get; }

        public TagDetectEventArgs(INfcTag tag)
        {
            Tag = tag;
        }
    }
}
