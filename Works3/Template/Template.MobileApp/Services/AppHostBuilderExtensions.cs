namespace Template.MobileApp.Services;

using System.Net.Http.Headers;

public static class AppHostBuilderExtensions
{
    public static MauiAppBuilder ConfigureHttpClient(this MauiAppBuilder builder)
    {
        builder.Services
            .AddHttpClient(ApiNames.Default, (p, client) =>
            {
                client.BaseAddress = p.GetRequiredService<ApiContext>().BaseAddress;
                client.Timeout = TimeSpan.FromSeconds(30);
                client.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
                client.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("deflate"));
            })
            .ConfigurePrimaryHttpMessageHandler(CreatePrimaryHandler)
            .AddHttpMessageHandler<ApiDelegatingHandler>();

        // 転送用: サイズ依存の処理時間に一律タイムアウトを適用しない。中断は呼び出し側のCancellationTokenで行う
        builder.Services
            .AddHttpClient(ApiNames.Transfer, (p, client) =>
            {
                client.BaseAddress = p.GetRequiredService<ApiContext>().BaseAddress;
                client.Timeout = Timeout.InfiniteTimeSpan;
                client.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
                client.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("deflate"));
            })
            .ConfigurePrimaryHttpMessageHandler(CreatePrimaryHandler)
            .AddHttpMessageHandler<ApiDelegatingHandler>();

        builder.Services.AddTransient<ApiDelegatingHandler>();

        builder.Services.AddSingleton<ApiContext>();

        return builder;
    }

    private static SocketsHttpHandler CreatePrimaryHandler() =>
        new()
        {
            AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
            PooledConnectionLifetime = TimeSpan.FromMinutes(1)
        };
}
