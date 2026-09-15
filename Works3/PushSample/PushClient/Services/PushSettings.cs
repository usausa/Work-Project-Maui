namespace PushClient.Services;

// 設定のキー (前景サービスの再起動時にも読めるよう Preferences に保存する)
public static class PushSettings
{
    public const string AddressKey = "server_address";

    public const string DefaultAddress = "http://localhost:5000";
}
