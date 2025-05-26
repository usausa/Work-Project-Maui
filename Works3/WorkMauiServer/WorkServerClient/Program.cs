using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace WorkServerClient;

using Microsoft.Extensions.Hosting;

using Rester;

using System.Net.Http.Headers;

public class Program
{
    public static async Task Main(string[] args)
    {
        RestConfig.Default.UseJsonSerializer();

        // builder
        var builder = Host.CreateApplicationBuilder(args);

        builder.Logging.ClearProviders();
        builder.Logging.AddSimpleConsole(options =>
        {
            options.SingleLine = true;
            options.TimestampFormat = "yyyy-MM-dd HH:mm:ss ";
        });

        builder.Services.AddSingleton<TokenAccessor>();
        builder.Services.AddTransient<JwtAuthHandler>();
        builder.Services
            .AddHttpClient("Api", client =>
            {
                client.BaseAddress = new Uri("http://127.0.0.1:5000");
            })
            .AddHttpMessageHandler<JwtAuthHandler>();

        // host
        var host = builder.Build();

        var log = host.Services.GetRequiredService<ILogger<Program>>();

        var httpClientFactory = host.Services.GetRequiredService<IHttpClientFactory>();
        using var client = httpClientFactory.CreateClient("Api");

        var response = await client.PostAsync<LoginResponse>("/api/account/login", new LoginRequest { Id = "test" });
        if (response.RestResult != RestResult.Success)
        {
            log.LogWarning("Login failed. result=[{Result}]", response.RestResult);
            return;
        }

        log.LogInformation("Login success. token=[{Token}]", response.Content!.Token);

        var tokenAccessor = host.Services.GetRequiredService<TokenAccessor>();
        tokenAccessor.Token = response.Content.Token;

        var response2 = await client.GetAsync<SecretMessageResponse>("/api/secret/message");
        if (response2.RestResult != RestResult.Success)
        {
            log.LogWarning("Request failed. result=[{Result}]", response2.RestResult);
            return;
        }

        log.LogInformation("Request success. message=[{Message}]", response2.Content?.Message);
    }
}

public class TokenAccessor
{
    public string Token { get; set; } = default!;
}

public class JwtAuthHandler : DelegatingHandler
{
    private readonly TokenAccessor tokenAccessor;

    public JwtAuthHandler(TokenAccessor tokenAccessor)
    {
        this.tokenAccessor = tokenAccessor;
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var token = tokenAccessor.Token;
        if (!String.IsNullOrEmpty(token))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        return base.SendAsync(request, cancellationToken);
    }
}

public class LoginRequest
{
    public string Id { get; set; } = default!;
}

public class LoginResponse
{
    public string Token { get; set; } = default!;
}

public class SecretMessageResponse
{
    public string Message { get; set; } = default!;
}
