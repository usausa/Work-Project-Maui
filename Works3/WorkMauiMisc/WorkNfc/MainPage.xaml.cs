namespace WorkNfc;

using Android.App;
using Android.Content;
using Android.Nfc;
using Android.Nfc.Tech;

using Application = Android.App.Application;

public partial class MainPage : ContentPage
{
    private readonly NfcReader reader;

    public MainPage()
    {
        InitializeComponent();

        reader = new NfcReader();
    }

    private void OnStartClicked(object? sender, EventArgs e)
    {
        reader.Start();
    }

    private void OnStopClicked(object? sender, EventArgs e)
    {
        reader.Stop();
    }
}

public partial class NfcReader
{
    // TODO
}

#if ANDROID
public partial class NfcReader
{
    private NfcAdapter? nfcAdapter;

    public void Start()
    {
        if (nfcAdapter is null)
        {
            var nfcManager = (NfcManager)Application.Context.GetSystemService(Context.NfcService)!;
            nfcAdapter = nfcManager.DefaultAdapter!;
        }

        var techLists = new List<string[]>();
        techLists.Add(["android.nfc.tech.NfcF"]);

        var activity = ActivityResolver.CurrentActivity;

        var intent = new Intent(activity, activity.GetType()).AddFlags(ActivityFlags.SingleTop);
        nfcAdapter.EnableForegroundDispatch(
            activity,
            PendingIntent.GetActivity(activity, 0, intent, PendingIntentFlags.Mutable),
            [new IntentFilter(NfcAdapter.ActionNdefDiscovered)],
            techLists.ToArray());
    }

    public void Stop()
    {
        var activity = ActivityResolver.CurrentActivity;

        nfcAdapter?.DisableForegroundDispatch(activity);
    }

    public static void ProcessIntent(Intent intent)
    {
        try
        {
            var idm = intent.GetByteArrayExtra(NfcAdapter.ExtraId)!;
            // TODO
            var tag = (Tag?)intent.GetParcelableExtra(NfcAdapter.ExtraTag);
            if (tag is null)
            {
                return;
            }

            var list = tag.GetTechList()!;
            if (list.Any(x => x == "android.nfc.tech.NfcF"))
            {
                var nfc = NfcF.Get(tag)!;
                nfc.Timeout = 1000;
                nfc.Connect();
            }
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

public static class ActivityResolver
{
    public static Activity CurrentActivity { get; set; } = default!;
}
#endif
