namespace Template.MobileApp.Services;

using System.Net.Http.Json;
using System.Text.Json;

using Rester;

public sealed class HttpService
{
    // PUT / DELETE は Rester に無いため HttpClient を直接使う。JSON はモバイル契約 (PascalCase) に合わせる
    private static readonly JsonSerializerOptions SerializerOptions = new() { PropertyNameCaseInsensitive = true };

    // IHttpClientFactoryが返すクライアントはハンドラがプール管理されるためDispose不要
    private readonly IHttpClientFactory httpClientFactory;

    public HttpService(IHttpClientFactory httpClientFactory)
    {
        this.httpClientFactory = httpClientFactory;
    }

    //--------------------------------------------------------------------------------
    // Basic
    //--------------------------------------------------------------------------------

    public ValueTask<IRestResponse<ServerTimeResponse>> GetServerTimeAsync(CancellationToken cancellationToken = default)
    {
        var client = httpClientFactory.CreateClient(ApiNames.Default);
        return client.GetAsync<ServerTimeResponse>("api/server/time", cancel: cancellationToken);
    }

    //--------------------------------------------------------------------------------
    // Data
    //--------------------------------------------------------------------------------

    public ValueTask<IRestResponse<DataListResponse>> GetDataListAsync(CancellationToken cancellationToken = default)
    {
        var client = httpClientFactory.CreateClient(ApiNames.Default);
        return client.GetAsync<DataListResponse>("api/data/list", cancel: cancellationToken);
    }

    // 先頭からの位置と件数で取得する (追加読み込み用)
    public ValueTask<IRestResponse<DataListResponse>> GetDataListAsync(int offset, int size, CancellationToken cancellationToken = default)
    {
        var client = httpClientFactory.CreateClient(ApiNames.Default);
        return client.GetAsync<DataListResponse>($"api/data/list?offset={offset}&size={size}", cancel: cancellationToken);
    }

    public ValueTask<IRestResponse<DataResponse>> GetDataAsync(long id, CancellationToken cancellationToken = default)
    {
        var client = httpClientFactory.CreateClient(ApiNames.Default);
        return client.GetAsync<DataResponse>($"api/data/{id}", cancel: cancellationToken);
    }

    public ValueTask<IRestResponse<DataCreateResponse>> PostDataAsync(DataCreateRequest request, CancellationToken cancellationToken = default)
    {
        var client = httpClientFactory.CreateClient(ApiNames.Default);
        return client.PostAsync<DataCreateResponse>("api/data", request, cancel: cancellationToken);
    }

    public ValueTask<IRestResponse<object>> PutDataAsync(long id, DataUpdateRequest request, CancellationToken cancellationToken = default)
    {
        var client = httpClientFactory.CreateClient(ApiNames.Default);
        return SendAsync<object>(() => client.PutAsJsonAsync($"api/data/{id}", request, SerializerOptions, cancellationToken), cancellationToken);
    }

    public ValueTask<IRestResponse<object>> DeleteDataAsync(long id, CancellationToken cancellationToken = default)
    {
        var client = httpClientFactory.CreateClient(ApiNames.Default);
        return SendAsync<object>(() => client.DeleteAsync(new Uri($"api/data/{id}", UriKind.Relative), cancellationToken), cancellationToken);
    }

    //--------------------------------------------------------------------------------
    // Secret
    //--------------------------------------------------------------------------------

    public ValueTask<IRestResponse<SecretMessageResponse>> GetSecretMessageAsync(CancellationToken cancellationToken = default)
    {
        var client = httpClientFactory.CreateClient(ApiNames.Default);
        return client.GetAsync<SecretMessageResponse>("api/secret/message", cancel: cancellationToken);
    }

    public ValueTask<IRestResponse<AccountLoginResponse>> PostAccountLoginAsync(AccountLoginRequest request, CancellationToken cancellationToken = default)
    {
        var client = httpClientFactory.CreateClient(ApiNames.Default);
        return client.PostAsync<AccountLoginResponse>("api/account/login", request, cancel: cancellationToken);
    }

    //--------------------------------------------------------------------------------
    // Storage
    //--------------------------------------------------------------------------------

    // ディレクトリの一覧 (パスは末尾 / 付き、ルートは空)
    public ValueTask<IRestResponse<StorageListResponse>> GetStorageListAsync(string path, CancellationToken cancellationToken = default)
    {
        var client = httpClientFactory.CreateClient(ApiNames.Default);
        return client.GetAsync<StorageListResponse>($"api/storage/{path}", cancel: cancellationToken);
    }

    public ValueTask<IRestResponse<object>> DeleteStorageAsync(string path, CancellationToken cancellationToken = default)
    {
        var client = httpClientFactory.CreateClient(ApiNames.Default);
        return SendAsync<object>(() => client.DeleteAsync(new Uri($"api/storage/{path}", UriKind.Relative), cancellationToken), cancellationToken);
    }

    public ValueTask<IRestResponse> DownloadAsync(string path, string filename, Action<double> action, CancellationToken cancellationToken = default)
    {
        var client = httpClientFactory.CreateClient(ApiNames.Transfer);
        return client.DownloadAsync(
            $"api/storage/{path}",
            filename,
            progress: CreateProgressCallback(action),
            cancel: cancellationToken);
    }

    public ValueTask<IRestResponse> DownloadAsync(string path, Stream stream, Action<double> action, CancellationToken cancellationToken = default)
    {
        var client = httpClientFactory.CreateClient(ApiNames.Transfer);
        return client.DownloadAsync(
            $"api/storage/{path}",
            stream,
            progress: CreateProgressCallback(action),
            cancel: cancellationToken);
    }

    public ValueTask<IRestResponse> UploadAsync(string path, string filename, Action<double> action, CancellationToken cancellationToken = default)
    {
        var client = httpClientFactory.CreateClient(ApiNames.Transfer);
        return client.UploadAsync(
            $"api/storage/{path}",
            filename,
            compress: CompressOption.Gzip,
            progress: CreateProgressCallback(action),
            cancel: cancellationToken);
    }

    // 画像など圧縮の効かないものは compress を false にする (Content-Length が付き進捗も出る)
    public ValueTask<IRestResponse> UploadAsync(string path, Stream stream, Action<double> action, bool compress, CancellationToken cancellationToken = default)
    {
        var client = httpClientFactory.CreateClient(ApiNames.Transfer);
        return client.UploadAsync(
            $"api/storage/{path}",
            stream,
            compress: compress ? CompressOption.Gzip : CompressOption.None,
            progress: CreateProgressCallback(action),
            cancel: cancellationToken);
    }

    private static Action<long, long> CreateProgressCallback(Action<double> action)
    {
        var progress = -1d;
        return (processed, total) =>
        {
            // Content-Length不明時は進捗を通知しない
            if (total <= 0)
            {
                return;
            }

            var percent = Math.Floor((double)processed / total * 100);
            if (percent > progress)
            {
                progress = percent;
                action(percent);
            }
        };
    }

    //--------------------------------------------------------------------------------
    // Test
    //--------------------------------------------------------------------------------

    public ValueTask<IRestResponse<object>> GetTestErrorAsync(int code, CancellationToken cancellationToken = default)
    {
        var client = httpClientFactory.CreateClient(ApiNames.Default);
        return client.GetAsync<object>($"api/test/error/{code}", cancel: cancellationToken);
    }

    public ValueTask<IRestResponse<object>> GetTestDelayAsync(int timeout, CancellationToken cancellationToken = default)
    {
        var client = httpClientFactory.CreateClient(ApiNames.Default);
        return client.GetAsync<object>($"api/test/delay/{timeout}", cancel: cancellationToken);
    }

    //--------------------------------------------------------------------------------
    // Helper
    //--------------------------------------------------------------------------------

    // HttpClient の応答を Rester の結果に揃える (NetworkOperator で同じ扱いにするため)
    private static async ValueTask<IRestResponse<T>> SendAsync<T>(Func<Task<HttpResponseMessage>> func, CancellationToken cancellationToken)
    {
        try
        {
            using var response = await func().ConfigureAwait(false);
            if (!response.IsSuccessStatusCode)
            {
                return new RestResponse<T>(RestResult.HttpError, response.StatusCode, null, default);
            }

            // 本文なし (204) は既定値
            var content = ((response.Content.Headers.ContentLength ?? 0) > 0) && (typeof(T) != typeof(object))
                ? await response.Content.ReadFromJsonAsync<T>(SerializerOptions, cancellationToken).ConfigureAwait(false)
                : default;
            return new RestResponse<T>(RestResult.Success, response.StatusCode, null, content);
        }
        catch (OperationCanceledException ex)
        {
            return new RestResponse<T>(cancellationToken.IsCancellationRequested ? RestResult.Cancel : RestResult.Timeout, 0, ex, default);
        }
        catch (HttpRequestException ex)
        {
            return new RestResponse<T>(RestResult.RequestError, ex.StatusCode ?? 0, ex, default);
        }
        catch (JsonException ex)
        {
            return new RestResponse<T>(RestResult.SerializeError, 0, ex, default);
        }
    }
}
