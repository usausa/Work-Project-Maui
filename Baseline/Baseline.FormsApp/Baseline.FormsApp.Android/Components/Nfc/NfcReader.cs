namespace Baseline.FormsApp.Droid.Components.Nfc
{
    using System;

    using Android.App;
    using Android.Content;
    using Android.Nfc;
    using Android.Nfc.Tech;

    using Baseline.FormsApp.Components.Nfc;

    public sealed class NfcReader : INfcReader
    {
        public event EventHandler<TagDetectEventArgs> Detected;

        private readonly Activity activity;

        private readonly NfcAdapter nfcAdapter;

        private readonly PendingIntent pendingIntent;

        private readonly IntentFilter[] filters;

        private readonly string[][] techLists;

        private bool enabled;

        public NfcReader(Activity activity)
        {
            this.activity = activity;
            nfcAdapter = NfcAdapter.GetDefaultAdapter(activity);
            pendingIntent = PendingIntent.GetActivity(activity, 0, new Intent(activity, activity.GetType()).AddFlags(ActivityFlags.SingleTop), 0);
            filters = new[] { new IntentFilter(NfcAdapter.ActionTechDiscovered) };
            techLists = new[] { new[] { "android.nfc.tech.NfcF" } };
        }

        public bool Open()
        {
            enabled = true;

            EnableDispatch();

            return true;
        }

        public void Close()
        {
            DisableDispatch();

            enabled = false;
        }

        public void Resume()
        {
            if (enabled)
            {
                EnableDispatch();
            }
        }

        public void Pause()
        {
            if (enabled)
            {
                DisableDispatch();
            }
        }

        private void EnableDispatch()
        {
            nfcAdapter.EnableForegroundDispatch(activity, pendingIntent, filters, techLists);
        }

        private void DisableDispatch()
        {
            nfcAdapter.DisableForegroundDispatch(activity);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ignore")]
        public void OnNewIntent(Intent intent)
        {
            if (intent.Action != NfcAdapter.ActionTechDiscovered)
            {
                return;
            }

            var tag = (Tag)intent.GetParcelableExtra(NfcAdapter.ExtraTag);
            var nfc = NfcF.Get(tag);
            try
            {
                nfc.Timeout = 50;
                nfc.Connect();
                Detected?.Invoke(this, new TagDetectEventArgs(new NfcTag(nfc)));
            }
            catch (TagLostException)
            {
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e);
            }
        }
    }
}