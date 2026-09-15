namespace OtelClient.Services;

// 設定のキー (Preferences)
public static class TelemetrySettings
{
    public const string EndpointKey = "otlp_endpoint";

    public const string MauiSpansKey = "otlp_maui_spans";

    public const string DeviceIdKey = "device_id";

    public const string DefaultEndpoint = "http://192.168.100.10:8080/";

    // 端末の識別子 (初回に生成して保存)
    public static string GetDeviceId()
    {
        var id = Preferences.Default.Get(DeviceIdKey, string.Empty);
        if (id.Length == 0)
        {
            id = Guid.NewGuid().ToString("N")[..12];
            Preferences.Default.Set(DeviceIdKey, id);
        }

        return id;
    }
}
