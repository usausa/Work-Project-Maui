namespace Baseline.FormsApp.Components.Nfc
{
    using System;
    using System.Reactive.Linq;

    public static class NfcReaderExtensions
    {
        public static IObservable<INfcTag> AsDetectedObservable(this INfcReader reader)
        {
            return Observable
                .FromEvent<EventHandler<TagDetectEventArgs>, INfcTag>(h => (s, e) => h(e.Tag), h => reader.Detected += h, h => reader.Detected -= h);
        }
    }
}
