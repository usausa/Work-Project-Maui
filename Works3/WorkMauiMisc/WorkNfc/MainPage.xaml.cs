namespace WorkNfc;

using Android.OS;

using System.Buffers.Binary;

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
        reader.Detected += ReaderOnDetected;
    }

    private void OnStartClicked(object? sender, EventArgs e)
    {
        reader.Start();
    }

    private void OnStopClicked(object? sender, EventArgs e)
    {
        reader.Stop();
    }

    private void ReaderOnDetected(object? sender, NfcEventArgs e)
    {
        var nfcF = e.Tag;

        //var idm = nfcF.ExecutePolling(unchecked((short)0x0003));
        var idm = nfcF.ExecutePolling(unchecked((short)0xFFFF));
        if (idm.Length == 0)
        {
            return;
        }

        var block = new ReadBlock { BlockNo = 0 };
        if (!nfcF.ExecuteReadWoe(idm, 0x008B, block))
        {
            return;
        }

        var blocks1 = Enumerable.Range(0, 8).Select(x => new ReadBlock { BlockNo = (byte)x }).ToArray();
        var blocks2 = Enumerable.Range(8, 8).Select(x => new ReadBlock { BlockNo = (byte)x }).ToArray();
        var blocks3 = Enumerable.Range(16, 4).Select(x => new ReadBlock { BlockNo = (byte)x }).ToArray();
        if (!nfcF.ExecuteReadWoe(idm, 0x090F, blocks1) ||
            !nfcF.ExecuteReadWoe(idm, 0x090F, blocks2) ||
            !nfcF.ExecuteReadWoe(idm, 0x090F, blocks3))
        {
            return;
        }

        var access = Suica.ConvertToAccessData(block.BlockData);

        System.Diagnostics.Debug.WriteLine($"Idm: {Convert.ToHexString(idm)}");
        System.Diagnostics.Debug.WriteLine($"Balance: {access.Balance}");
        foreach (var data in blocks1.Concat(blocks2).Concat(blocks3).Select(x => Suica.ConvertToLogData(x.BlockData)))
        {
            System.Diagnostics.Debug.WriteLine($"{data!.DateTime:yyyy/MM/dd HH:mm:ss} {data.Balance}");
        }
    }
}

public sealed class NfcEventArgs : EventArgs
{
    public INfc Tag { get; }

    public NfcEventArgs(INfc tag)
    {
        Tag = tag;
    }
}

public partial class NfcReader
{
    public event EventHandler<NfcEventArgs>? Detected;
}

#if ANDROID
public partial class NfcReader : Java.Lang.Object, NfcAdapter.IReaderCallback, Application.IActivityLifecycleCallbacks
{
    private NfcAdapter? nfcAdapter;

    private Activity? currentActivity;

    private bool enabled;

    public void Start()
    {
        if (nfcAdapter is null)
        {
            var nfcManager = (NfcManager)Application.Context.GetSystemService(Context.NfcService)!;
            nfcAdapter = nfcManager.DefaultAdapter!;
        }

        currentActivity = ActivityResolver.CurrentActivity;
        currentActivity.Application!.RegisterActivityLifecycleCallbacks(this);

        nfcAdapter.EnableReaderMode(currentActivity, this, NfcReaderFlags.NfcF, null);

        enabled = true;
    }

    public void Stop()
    {
        if (!enabled)
        {
            return;
        }

        nfcAdapter?.DisableReaderMode(currentActivity);
        currentActivity = null;

        enabled = false;
    }

    private void Pause()
    {
        if (!enabled)
        {
            return;
        }

        nfcAdapter?.DisableReaderMode(currentActivity);
    }

    private void Resume()
    {
        if (!enabled)
        {
            return;
        }

        nfcAdapter?.EnableReaderMode(currentActivity, this, NfcReaderFlags.NfcF, null);
    }

    public void OnTagDiscovered(Tag? tag)
    {
        try
        {
            if (tag is null)
            {
                return;
            }

            var list = tag.GetTechList()!;
            if (list.Any(x => x == "android.nfc.tech.NfcF"))
            {
                var id = tag.GetId()!;
                var nfc = NfcF.Get(tag)!;
                Detected?.Invoke(this, new NfcEventArgs(new AndroidNfcF(id, nfc)));
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

    // --------------------------------------------------------------------------------
    // --------------------------------------------------------------------------------

    public void OnActivityCreated(Activity activity, Bundle? savedInstanceState)
    {
    }

    public void OnActivityDestroyed(Activity activity)
    {
    }

    public void OnActivityPaused(Activity activity)
    {
        Pause();
    }

    public void OnActivityResumed(Activity activity)
    {
        Resume();
    }

    public void OnActivitySaveInstanceState(Activity activity, Bundle outState)
    {
    }

    public void OnActivityStarted(Activity activity)
    {
    }

    public void OnActivityStopped(Activity activity)
    {
    }
}

public static class Suica
{
    // --------------------------------------------------------------------------------
    // Helpers
    // --------------------------------------------------------------------------------

    public static SuicaAccessData ConvertToAccessData(byte[] data)
    {
        return new SuicaAccessData
        {
            Balance = BinaryPrimitives.ReadUInt16LittleEndian(data.AsSpan(11, 2)),
            TransactionId = BinaryPrimitives.ReadUInt16BigEndian(data.AsSpan(14, 2))
        };
    }

    public static SuicaLogData? ConvertToLogData(byte[] data)
    {
        if (data[1] == 0x00)
        {
            return null;
        }

        return new SuicaLogData
        {
            Terminal = data[0],
            Process = data[1],
            DateTime = IsProcessOfSales(data[1]) ? ConvertDateTime(data, 4) : ConvertDate(data, 4),
            Balance = BinaryPrimitives.ReadUInt16LittleEndian(data.AsSpan(10, 2)),
            TransactionId = BinaryPrimitives.ReadUInt16BigEndian(data.AsSpan(13, 2))
        };
    }

    private static readonly HashSet<byte> ProcessOfSales = [70, 72, 73, 74, 75];

    public static bool IsProcessOfSales(byte process)
    {
        var processType = ConvertProcessType(process);
        return ProcessOfSales.Contains(processType);
    }

    public static byte ConvertProcessType(byte process)
    {
        return (byte)(process & 0b01111111);
    }
    public static DateTime ConvertDate(byte[] bytes, int offset)
    {
        var year = 2000 + (bytes[offset] >> 1);
        var month = BinaryPrimitives.ReadUInt16BigEndian(bytes.AsSpan(offset, 2)) >> 5 & 0b1111;
        var day = bytes[offset + 1] & 0b11111;
        return new DateTime(year, month, day);
    }

    public static DateTime ConvertDateTime(byte[] bytes, int offset)
    {
        var year = 2000 + (bytes[offset] >> 1);
        var month = BinaryPrimitives.ReadUInt16BigEndian(bytes.AsSpan(offset, 2)) >> 5 & 0b1111;
        var day = bytes[offset + 1] & 0b11111;
        var hour = bytes[offset + 2] >> 3;
        var minute = BinaryPrimitives.ReadUInt16BigEndian(bytes.AsSpan(offset + 2, 2)) >> 5 & 0b111111;
        return new DateTime(year, month, day, hour, minute, 0);
    }
}

public class SuicaAccessData
{
    public int Balance { get; set; }

    public int TransactionId { get; set; }
}

public class SuicaLogData
{
    public byte Terminal { get; set; }

    public byte Process { get; set; }

    public DateTime DateTime { get; set; }

    public int Balance { get; set; }

    public int TransactionId { get; set; }
}

public static class ActivityResolver
{
    public static Activity CurrentActivity { get; set; } = default!;
}
#endif

#pragma warning disable CA1819
public interface INfc
{
    byte[] Id { get; }

    void SetTimeout(int timeout);

    byte[] Access(byte[] command);
}
#pragma warning restore CA1819

public sealed class AndroidNfcF : INfc
{
    public byte[] Id { get; }

    private readonly NfcF nfc;

    public AndroidNfcF(byte[] id, NfcF nfc)
    {
        Id = id;
        this.nfc = nfc;
    }

    public void SetTimeout(int timeout)
    {
        nfc.Timeout = timeout;
    }

    public byte[] Access(byte[] command)
    {
        if (!nfc.IsConnected)
        {
            nfc.Connect();
        }

        try
        {
            var response = nfc.Transceive(command);
            return response ?? [];
        }
        catch (TagLostException)
        {
            return [];
        }
    }
}

#pragma warning disable CA1819
public sealed class ReadBlock
{
    public byte BlockNo { get; set; }

    public byte[] BlockData { get; set; } = default!;
}
#pragma warning restore CA1819

public static class FeliCaExtensions
{
    public static byte[] ExecutePolling(this INfc nfc, short systemCode)
    {
        var command = new byte[6];
        command[0] = (byte)command.Length;
        command[1] = 0x00;
        command[2] = (byte)(systemCode >> 8);
        command[3] = (byte)(systemCode & 0xFF);
        command[4] = 0x01;
        command[5] = 0x00;

        var response = nfc.Access(command);
        if (response.Length < 18)
        {
            return [];
        }

        return response.SubArray(2, 8);
    }

    public static bool ExecuteReadWoe(this INfc nfc, byte[] idm, short serviceCode, params ReadBlock[] blocks)
    {
        var command = new byte[14 + (blocks.Length * 2)];
        command[0] = (byte)command.Length;
        command[1] = 0x06;
        Buffer.BlockCopy(idm, 0, command, 2, idm.Length);
        command[10] = 1;
        command[11] = (byte)(serviceCode & 0xff);
        command[12] = (byte)(serviceCode >> 8);
        command[13] = (byte)blocks.Length;
        for (var i = 0; i < blocks.Length; i++)
        {
            var offset = 14 + (i * 2);
            command[offset] = 0x80;
            command[offset + 1] = blocks[i].BlockNo;
        }

        var response = nfc.Access(command);
        if (response.Length < 12)
        {
            return false;
        }

        if ((response[10] != 0x00) || (response[11] != 0x00))
        {
            return false;
        }

        if (response.Length < (13 + (response[12] * 16)))
        {
            return false;
        }

        for (var i = 0; i < blocks.Length; i++)
        {
            var offset = 13 + (16 * i);
            blocks[i].BlockData = response.SubArray(offset, 16);
        }

        return true;
    }

    private static byte[] SubArray(this byte[] array, int offset, int length)
    {
        var bytes = new byte[Math.Min(length, array.Length - offset)];
        Buffer.BlockCopy(array, offset, bytes, 0, bytes.Length);
        return bytes;
    }
}
