namespace Template.MobileApp.Services;

using Rester;

public sealed class NetworkService : IDisposable
{
    private HttpClient client;

    private readonly Dictionary<string, object> headers = new();

    public NetworkService()
    {
        client = CreateHttpClient();
    }

    public void Dispose()
    {
        client.Dispose();
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:DisposeObjectsBeforeLosingScope", Justification = "Ignore")]
    private static HttpClient CreateHttpClient()
    {
        return new(new HttpClientHandler
        {
            AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
            ServerCertificateCustomValidationCallback = static (_, _, _, _) => true
        })
        {
            Timeout = new TimeSpan(0, 0, 0, 30)
        };
    }

    public void SetEndPoint(string address)
    {
        if (client.BaseAddress is not null)
        {
            client.Dispose();
            client = CreateHttpClient();
        }

        client.BaseAddress = String.IsNullOrEmpty(address) ? null : new Uri(address);
    }

    public void SetToken(string token)
    {
        headers["X-API-Token"] = token;
    }

    //--------------------------------------------------------------------------------
    // Basic
    //--------------------------------------------------------------------------------

    public ValueTask<IRestResponse<ServerTimeResponse>> GetServerTimeAsync() =>
        client.GetAsync<ServerTimeResponse>(
            "api/server/time",
            headers);

    //--------------------------------------------------------------------------------
    // Test
    //--------------------------------------------------------------------------------

    public ValueTask<IRestResponse<object>> GetTestErrorAsync(int code) =>
        client.GetAsync<object>(
            $"api/test/error/{code}",
            headers);

    public ValueTask<IRestResponse<object>> GetTestDelayAsync(int timeout) =>
        client.GetAsync<object>(
            $"api/test/delay/{timeout}",
            headers);

    //--------------------------------------------------------------------------------
    // Data
    //--------------------------------------------------------------------------------

    public ValueTask<IRestResponse<DataListResponse>> GetDataListAsync() =>
        client.GetAsync<DataListResponse>(
            "api/data/list",
            headers);

    //--------------------------------------------------------------------------------
    // Storage
    //--------------------------------------------------------------------------------

    // TODO
}
