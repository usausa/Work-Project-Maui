#pragma warning disable IDE0130
// ReSharper disable once CheckNamespace
namespace PushClient.Services;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;

using AndroidX.Core.App;

// 接続を常駐させる前景サービス。強制終了されると Sticky で再起動される (Intent は null なので接続先は Preferences から読む)
// 接続 (PushConnection) は DI のシングルトンで、サービスは所有しない
[Service(Name = "pushsample.client.PushService", Exported = false)]
public sealed class PushService : Service
{
    public const string ExtraAddress = "address";

    private const int NotificationId = 1;

    private bool subscribed;

    public override IBinder? OnBind(Intent? intent) => null;

    public override StartCommandResult OnStartCommand(Intent? intent, StartCommandFlags flags, int startId)
    {
        var services = IPlatformApplication.Current?.Services;
        if (services is null)
        {
            StopSelf();
            return StartCommandResult.NotSticky;
        }

        // 前景化 (Android 14 以降は種別付き)。バックグラウンドからの起動制限に掛かったときは諦める
        try
        {
            using var notification = Notifier.BuildStatusNotification(this, "接続を開始しています");
            if (OperatingSystem.IsAndroidVersionAtLeast(34))
            {
                StartForeground(NotificationId, notification, ForegroundService.TypeSpecialUse);
            }
            else
            {
                StartForeground(NotificationId, notification);
            }
        }
        catch (Java.Lang.IllegalStateException ex)
        {
            services.GetRequiredService<ILogger<PushService>>().WarnForegroundNotAllowed(ex);
            StopSelf();
            return StartCommandResult.NotSticky;
        }

        var address = intent?.GetStringExtra(ExtraAddress) ?? Preferences.Default.Get(PushSettings.AddressKey, string.Empty);
        if (String.IsNullOrEmpty(address))
        {
            StopSelf();
            return StartCommandResult.NotSticky;
        }

        var connection = services.GetRequiredService<PushConnection>();
        if (!subscribed)
        {
            connection.StatusChanged += OnStatusChanged;
            subscribed = true;
        }

        // 接続ループは自分で再試行するため待たない (例外は内部で処理される)
        _ = connection.StartAsync(address);
        return StartCommandResult.Sticky;
    }

    public override void OnDestroy()
    {
        var connection = IPlatformApplication.Current?.Services.GetService<PushConnection>();
        if (connection is not null)
        {
            if (subscribed)
            {
                connection.StatusChanged -= OnStatusChanged;
                subscribed = false;
            }

            _ = connection.StopAsync();
        }

        base.OnDestroy();
    }

    // 常駐通知に接続の状態を出す
    private void OnStatusChanged(object? sender, EventArgs e)
    {
        if (sender is not PushConnection connection)
        {
            return;
        }

        var text = connection.Status switch
        {
            PushStatus.Connected => "接続中",
            PushStatus.Connecting => "接続を試みています",
            PushStatus.Reconnecting => "再接続中",
            _ => "停止"
        };
        if (!String.IsNullOrEmpty(connection.LastError))
        {
            text = $"{text} ({connection.LastError})";
        }

        using var notification = Notifier.BuildStatusNotification(this, text);
        using var compat = NotificationManagerCompat.From(this);
        compat?.Notify(NotificationId, notification);
    }
}
