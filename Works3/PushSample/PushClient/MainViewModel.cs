namespace PushClient;

using System.Collections.ObjectModel;
using System.Net.Http.Json;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using PushClient.Services;

using PushShared;

// 接続の状態と受信した通知を表示し、前景サービスの開始 / 停止とテスト送信を行う
public sealed partial class MainViewModel : ObservableObject
{
    private static readonly HttpClient HttpClient = new();

    private readonly PushConnection connection;

    private readonly IPushHost host;

    private readonly INotifier notifier;

    [ObservableProperty]
    public partial string ServerAddress { get; set; } = Preferences.Default.Get(PushSettings.AddressKey, PushSettings.DefaultAddress);

    [ObservableProperty]
    public partial string Status { get; set; } = "停止";

    [ObservableProperty]
    public partial string ConnectionId { get; set; } = "-";

    [ObservableProperty]
    public partial string LastError { get; set; } = string.Empty;

    [ObservableProperty]
    public partial bool HasError { get; set; }

    public ObservableCollection<PushMessage> Messages { get; } = [];

    public MainViewModel(PushConnection connection, IPushHost host, INotifier notifier)
    {
        this.connection = connection;
        this.host = host;
        this.notifier = notifier;

        // 接続側のイベントはバックグラウンドスレッドから来る
        connection.StatusChanged += (_, _) => MainThread.BeginInvokeOnMainThread(UpdateStatus);
        connection.MessageReceived += (_, e) => MainThread.BeginInvokeOnMainThread(() => Messages.Insert(0, e.Message));
        UpdateStatus();
    }

    // 通知の許可を得てから前景サービスを起動する (許可が無くても接続はするが常駐通知が見えない)
    [RelayCommand]
    private async Task StartAsync()
    {
        var address = ServerAddress.Trim();
        if (!Uri.TryCreate(address, UriKind.Absolute, out _))
        {
            SetError("接続先の URL が不正です");
            return;
        }

        if (!await notifier.RequestPermissionAsync().ConfigureAwait(true))
        {
            SetError("通知の許可がありません (常駐通知と受信通知が表示されません)");
        }

        Preferences.Default.Set(PushSettings.AddressKey, address);
        host.StartService(address);
    }

    [RelayCommand]
    private void Stop() => host.StopService();

    // サーバの配信 API を叩いて自分に届くことを確かめる
    [RelayCommand]
    private async Task SendTestAsync()
    {
        try
        {
            var request = new PushRequest("テスト", $"送信 {DateTime.Now:HH:mm:ss}");
            using var response = await HttpClient.PostAsJsonAsync(new Uri(new Uri(ServerAddress.Trim()), "/push"), request, PushJsonContext.Default.PushRequest).ConfigureAwait(true);
            response.EnsureSuccessStatusCode();
            SetError(string.Empty);
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException or UriFormatException)
        {
            SetError(ex.Message);
        }
    }

    [RelayCommand]
    private void OpenBatterySettings() => host.OpenBatterySettings();

    private void UpdateStatus()
    {
        Status = connection.Status switch
        {
            PushStatus.Connecting => "接続中...",
            PushStatus.Connected => "接続済み",
            PushStatus.Reconnecting => "再接続中...",
            _ => "停止"
        };
        ConnectionId = connection.ConnectionId ?? "-";
        SetError(connection.LastError ?? string.Empty);
    }

    private void SetError(string message)
    {
        LastError = message;
        HasError = message.Length > 0;
    }
}
