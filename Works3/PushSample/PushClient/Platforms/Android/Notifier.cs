#pragma warning disable IDE0130
// ReSharper disable once CheckNamespace
namespace PushClient.Services;

using Android.App;
using Android.Content;

using AndroidX.Core.App;

using PushShared;

using AndroidNotificationManager = Android.App.NotificationManager;

// 常駐通知 (前景サービス用、音なし) と受信通知の 2 チャンネル
public sealed class Notifier : INotifier
{
    private const string StatusChannelId = "push.status";

    private const string MessageChannelId = "push.message";

    private static int messageId = 100;

    public async Task<bool> RequestPermissionAsync()
    {
        var status = await Permissions.CheckStatusAsync<Permissions.PostNotifications>().ConfigureAwait(false);
        if (status != PermissionStatus.Granted)
        {
            status = await Permissions.RequestAsync<Permissions.PostNotifications>().ConfigureAwait(false);
        }

        return status == PermissionStatus.Granted;
    }

    public void Show(PushMessage message)
    {
        var context = Application.Context;
        EnsureChannels(context);

        using var contentIntent = CreateContentIntent(context);
        using var builder = new NotificationCompat.Builder(context, MessageChannelId);
        builder.SetSmallIcon(_Microsoft.Android.Resource.Designer.ResourceConstant.Mipmap.appicon);
        builder.SetContentTitle(message.Title);
        builder.SetContentText(message.Body);
        builder.SetWhen(message.SentAt.ToUnixTimeMilliseconds());
        builder.SetPriority(NotificationCompat.PriorityDefault);
        builder.SetAutoCancel(true);
        builder.SetContentIntent(contentIntent);

        using var notification = builder.Build();
        using var compat = NotificationManagerCompat.From(context);
        compat?.Notify(Interlocked.Increment(ref messageId), notification);
    }

    // 前景サービスの常駐通知 (状態の文言だけ差し替える)
    public static Notification BuildStatusNotification(Context context, string text)
    {
        EnsureChannels(context);

        using var contentIntent = CreateContentIntent(context);
        using var builder = new NotificationCompat.Builder(context, StatusChannelId);
        builder.SetSmallIcon(_Microsoft.Android.Resource.Designer.ResourceConstant.Mipmap.appicon);
        builder.SetContentTitle("Push sample");
        builder.SetContentText(text);
        builder.SetOngoing(true);
        builder.SetSilent(true);
        builder.SetOnlyAlertOnce(true);
        builder.SetContentIntent(contentIntent);
        return builder.Build()!;
    }

    private static void EnsureChannels(Context context)
    {
        var manager = (AndroidNotificationManager?)context.GetSystemService(Context.NotificationService);
        if (manager is null)
        {
            return;
        }

        using var status = new NotificationChannel(StatusChannelId, "接続状態", NotificationImportance.Low);
        manager.CreateNotificationChannel(status);
        using var message = new NotificationChannel(MessageChannelId, "受信通知", NotificationImportance.Default);
        manager.CreateNotificationChannel(message);
    }

    private static PendingIntent CreateContentIntent(Context context)
    {
        using var intent = new Intent(context, typeof(MainActivity));
        return PendingIntent.GetActivity(context, 0, intent, PendingIntentFlags.UpdateCurrent | PendingIntentFlags.Immutable)!;
    }
}
