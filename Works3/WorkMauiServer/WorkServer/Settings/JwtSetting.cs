namespace WorkServer.Settings;

public sealed class JwtSetting
{
    public string Audience { get; set; } = default!;

    public string Issuer { get; set; } = default!;

    public string SecretKey { get; set; } = default!;

    public int ExpireDays { get; set; } = default!;
}
