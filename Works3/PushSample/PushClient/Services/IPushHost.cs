namespace PushClient.Services;

// 接続を常駐させるプラットフォーム側の入れ物 (Android は前景サービス)
public interface IPushHost
{
    void StartService(string address);

    void StopService();

    // 電池の最適化の設定を開く (常駐を殺されにくくするため、ユーザーが除外を選ぶ)
    void OpenBatterySettings();
}
