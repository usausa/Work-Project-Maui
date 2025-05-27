using System.Diagnostics;

namespace WorkHttpClient;
using Microsoft.Extensions.Logging;

using System.Net.Http.Headers;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        builder.Services
            .AddHttpClient("JwtClient", (p, client) =>
            {
                Debug.WriteLine("**** AddHttpClient");
                client.BaseAddress = new Uri(p.GetRequiredService<ClientContextAccessor>().BaseAddress);
                client.Timeout = TimeSpan.FromSeconds(30);
                client.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
                client.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("deflate"));
            })
            .ConfigurePrimaryHttpMessageHandler(() =>
            {
                Debug.WriteLine("**** ConfigurePrimaryHttpMessageHandler");
                var handler = new SocketsHttpHandler
                {
                    AutomaticDecompression = System.Net.DecompressionMethods.GZip | System.Net.DecompressionMethods.Deflate,
                    PooledConnectionLifetime = TimeSpan.FromMinutes(1),
                };
                handler.SslOptions.RemoteCertificateValidationCallback = static (_, _, _, _) => true;
                return handler;
            })
            .AddHttpMessageHandler<JwtAuthenticationHandler>();
        builder.Services.AddTransient<JwtAuthenticationHandler>();


        builder.Services.AddSingleton<ClientContextAccessor>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}

public sealed class ClientContextAccessor
{
    public string BaseAddress { get; set; } = default!;

    public string Token { get; set; } = default!;
}

public sealed class JwtAuthenticationHandler : DelegatingHandler
{
    private readonly ClientContextAccessor contextAccessor;

    public JwtAuthenticationHandler(ClientContextAccessor contextAccessor)
    {
        Debug.WriteLine("**** JwtAuthenticationHandler create");
        this.contextAccessor = contextAccessor;
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        Debug.WriteLine("**** SendAsync");

        var token = contextAccessor.Token;
        if (!String.IsNullOrEmpty(token))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
        return base.SendAsync(request, cancellationToken);
    }
}
