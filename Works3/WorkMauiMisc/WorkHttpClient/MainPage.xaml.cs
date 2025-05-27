using System.Diagnostics;

namespace WorkHttpClient;

public partial class MainPage : ContentPage
{
    private readonly IHttpClientFactory httpClientFactory;

    private readonly ClientContextAccessor contextAccessor;

    public MainPage(IServiceProvider services)
    {
        InitializeComponent();

        httpClientFactory = services.GetRequiredService<IHttpClientFactory>();
        contextAccessor = services.GetRequiredService<ClientContextAccessor>();
        contextAccessor.BaseAddress = "http://127.0.0.1:5000";
    }

    private async void Button_OnClicked(object? sender, EventArgs e)
    {
        using var client = httpClientFactory.CreateClient("JwtClient");
        try
        {
            var result = await client.GetStringAsync("api/server/time");
            Debug.WriteLine(result);
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
        }
    }
}
