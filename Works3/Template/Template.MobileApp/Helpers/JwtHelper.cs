namespace Template.MobileApp.Helpers;

using System.Text.Json;

public static class JwtHelper
{
    // JWT のペイロード (base64url) から有効期限 (exp) を取り出す。署名は検証しない (表示と再ログインの判定用)
    public static DateTime? GetExpiration(string token)
    {
        var parts = token.Split('.');
        if (parts.Length < 2)
        {
            return null;
        }

        try
        {
            var payload = parts[1].Replace('-', '+').Replace('_', '/');
            payload = payload.PadRight(payload.Length + ((4 - (payload.Length % 4)) % 4), '=');
            using var document = JsonDocument.Parse(Convert.FromBase64String(payload));
            if (document.RootElement.TryGetProperty("exp", out var exp) && exp.TryGetInt64(out var seconds))
            {
                return DateTimeOffset.FromUnixTimeSeconds(seconds).LocalDateTime;
            }
        }
        catch (Exception ex) when (ex is FormatException or JsonException)
        {
            // 不正なトークンは有効期限なしとして扱う
        }

        return null;
    }
}
