namespace PushShared;

// サーバからクライアントへ配信する通知 1 件
public sealed record PushMessage(string Title, string Body, DateTimeOffset SentAt);

// ハブの接続先とメソッド名 (サーバとクライアントで共有する)
public static class PushHubInfo
{
    public const string Path = "/hubs/push";

    public const string ReceiveMethod = "Receive";
}
