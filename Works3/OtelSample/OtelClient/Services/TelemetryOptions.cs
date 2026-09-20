namespace OtelClient.Services;

// 送信の設定。適用のたびにプロバイダーを作り直す。gRPC のときの API (api/time) はサーバの HTTP ポートへ
public sealed record TelemetryOptions(Uri Endpoint, bool UseGrpc, bool IncludeMauiSpans)
{
    public Uri ApiEndpoint => UseGrpc ? new UriBuilder(Endpoint) { Port = TelemetrySettings.HttpPort }.Uri : Endpoint;
}
