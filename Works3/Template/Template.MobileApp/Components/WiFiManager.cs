namespace Template.MobileApp.Components;

// 接続中の Wi-Fi の情報 (未接続は null)
public sealed record WiFiConnection(
    string Ssid,
    string Bssid,
    int Rssi,
    int SignalLevel,
    int MaxSignalLevel,
    int LinkSpeed,
    int RxLinkSpeed,
    int TxLinkSpeed,
    int Frequency,
    string Standard,
    string IpAddress,
    string Gateway,
    string Dns);

public enum WiFiSecurity
{
    Open,
    Wep,
    Wpa,
    Wpa2,
    Wpa3,
    Enterprise
}

// スキャンで検出したアクセスポイント
public sealed record WiFiAccessPoint(
    string Ssid,
    string Bssid,
    int Rssi,
    int SignalLevel,
    int Frequency,
    int Channel,
    int ChannelWidth,
    WiFiSecurity Security,
    string Standard,
    DateTime SeenAt);

public interface IWiFiManager : IDisposable
{
    /// <summary>
    /// 接続・切断・電波状況・スキャン結果の変化でUIスレッド以外から発火する。UI更新時はObserveOnCurrentContext等でマーシャリングすること。
    /// </summary>
    event EventHandler<EventArgs>? StateChanged;

    // 非搭載端末ではfalse
    bool IsSupported { get; }

    // Wi-Fi の無線が有効か
    bool IsRadioOn { get; }

    // 監視中の最新の接続情報。未接続または監視停止中は null
    WiFiConnection? Connection { get; }

    // 直近のスキャン結果 (電波の強い順)
    IReadOnlyList<WiFiAccessPoint> AccessPoints { get; }

    // 監視の開始 / 停止
    bool Enabled { get; set; }

    // スキャンを要求する (システムの制限で受け付けられないと false。結果は StateChanged で通知)
    bool StartScan();

    void OpenSettings();
}

public sealed partial class WiFiManager : IWiFiManager
{
    public event EventHandler<EventArgs>? StateChanged;

    public partial bool IsSupported { get; }

    public partial bool IsRadioOn { get; }

    public WiFiConnection? Connection { get; private set; }

    public IReadOnlyList<WiFiAccessPoint> AccessPoints { get; private set; } = [];

    public bool Enabled
    {
        get;
        set
        {
            if (value)
            {
                if (!field)
                {
                    Start();
                    field = true;
                }
            }
            else
            {
                if (field)
                {
                    Stop();
                    field = false;
                }
            }
        }
    }

    public void Dispose()
    {
        Enabled = false;
    }

    public partial bool StartScan();

    public partial void OpenSettings();

    private partial void Start();

    private partial void Stop();

    private void Update(WiFiConnection? connection)
    {
        Connection = connection;
        StateChanged?.Invoke(this, EventArgs.Empty);
    }

    private void UpdateAccessPoints(IReadOnlyList<WiFiAccessPoint> accessPoints)
    {
        AccessPoints = accessPoints;
        StateChanged?.Invoke(this, EventArgs.Empty);
    }
}
