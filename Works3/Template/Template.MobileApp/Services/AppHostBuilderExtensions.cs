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

        // 転送用: 大きいファイルを通常APIの30秒で切らないよう長めにする。
        // 無限にすると無応答時に中断手段が無くなるため上限は設ける (呼び出し側がCancellationTokenを渡す場合はそちらが優先)
        builder.Services
            .AddHttpClient(ApiNames.Transfer, (p, client) =>
            {
                client.BaseAddress = p.GetRequiredService<ApiContext>().BaseAddress;
                client.Timeout = TimeSpan.FromMinutes(10);
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
