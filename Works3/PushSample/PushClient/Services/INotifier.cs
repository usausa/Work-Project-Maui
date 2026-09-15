namespace PushClient.Services;

using PushShared;

public interface INotifier
{
    // Android 13 以降の通知の表示許可
    Task<bool> RequestPermissionAsync();

    // 受信した通知を表示する
    void Show(PushMessage message);
}
