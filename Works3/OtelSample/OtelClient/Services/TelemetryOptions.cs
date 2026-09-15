namespace OtelClient.Services;

// 送信の設定。適用のたびにプロバイダーを作り直す
public sealed record TelemetryOptions(Uri Endpoint, bool IncludeMauiSpans);
