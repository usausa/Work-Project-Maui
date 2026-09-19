namespace Template.MobileApp.Models.Api;

// SignalR ハブ (MonitorHub) の送受信メッセージ。サーバー側の Models/Api/MonitorMessages.cs と同じ形

// 端末 → サーバー: 端末の状態
public sealed class MonitorDeviceStatus
{
    public string DeviceId { get; set; } = default!;

    public string Model { get; set; } = default!;

    public string Platform { get; set; } = default!;

    public double Battery { get; set; }

    public string BatteryState { get; set; } = default!;

    public string Network { get; set; } = default!;
}

// サーバー → 端末: サーバーの状態 (1 秒ごと)
public sealed class MonitorServerStatus
{
    public DateTimeOffset Time { get; set; }

    public double CpuPercent { get; set; }

    public long WorkingSet { get; set; }

    public int Connections { get; set; }
}

// サーバー → 端末: 通知
public sealed class MonitorNotification
{
    public string Title { get; set; } = default!;

    public string Body { get; set; } = default!;

    public DateTimeOffset SentAt { get; set; }
}
