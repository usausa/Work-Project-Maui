using System.Buffers.Binary;

namespace WorkNfc;

using Android.App;
using Android.App.Slices;
using Android.Content;
using Android.Nfc;
using Android.Nfc.Tech;

using System.ComponentModel;

using static Android.Icu.Text.IDNA;

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
        // TODO 再取得しない形にする？
        var activity = ActivityResolver.CurrentActivity;

        nfcAdapter?.DisableForegroundDispatch(activity);
    }

    public static void ProcessIntent(Intent intent)
    {
        try
        {
            var id = intent.GetByteArrayExtra(NfcAdapter.ExtraId)!;
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
                //nfc.Timeout = 1000;
                nfc.Connect();

                var nfcF = new AndroidNfcF(id, nfc);

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

                var access = ConvertToAccessData(block.BlockData);

                System.Diagnostics.Debug.WriteLine($"Idm: {Convert.ToHexString(idm)}");
                System.Diagnostics.Debug.WriteLine($"Balance: {access.Balance}");
                foreach (var data in blocks1.Concat(blocks2).Concat(blocks3).Select(x => ConvertToLogData(x.BlockData)))
                {
                    System.Diagnostics.Debug.WriteLine($"{data!.DateTime:yyyy/MM/dd HH:mm:ss} {data.Balance}");
                }
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

    private static readonly HashSet<byte> ProcessOfSales = new(new byte[] { 70, 72, 73, 74, 75 });

    private static readonly HashSet<byte> ProcessOfBus = new(new byte[] { 13, 15, 31, 35 });

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

    byte[] Access(byte[] command);
}
#pragma warning restore CA1819

public sealed class AndroidNfcF : INfc
{
    private readonly NfcF nfc;

    public byte[] Id { get; }

    public AndroidNfcF(byte[] id, NfcF nfc)
    {
        Id = id;
        this.nfc = nfc;
    }

    public byte[] Access(byte[] command)
    {
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
