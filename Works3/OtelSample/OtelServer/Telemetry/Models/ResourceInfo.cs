namespace OtelServer.Telemetry.Models;

// リソース属性 (service.name / device.* / os.* など)
public sealed record ResourceInfo(IReadOnlyList<KeyValueAttr> Attributes)
{
    public const string ServiceNameKey = "service.name";

    public const string ServiceVersionKey = "service.version";

    public const string DeviceIdKey = "device.id";

    public const string DeviceModelKey = "device.model";

    public const string OsNameKey = "os.name";

    public const string OsVersionKey = "os.version";

    public string ServiceName => GetAttribute(ServiceNameKey) ?? "unknown";

    public string DeviceId => GetAttribute(DeviceIdKey) ?? string.Empty;

    public string? GetAttribute(string key) => Attributes.FirstOrDefault(x => x.Key == key)?.Value;
}
