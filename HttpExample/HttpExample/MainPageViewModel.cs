namespace HttpExample;

using System.Windows.Input;

using Smart.Diagnostics;
using Smart.Maui.ViewModels;

using Rester;

public class MainPageViewModel : ViewModelBase
{
    private readonly IHttpClientFactory httpClientFactory;

    public ICommand GetCommand { get; }
    public ICommand PostCommand { get; }

    public ICommand Get2Command { get; }
    public ICommand Post2Command { get; }

    public MainPageViewModel(IHttpClientFactory httpClientFactory)
    {
        this.httpClientFactory = httpClientFactory;
        GetCommand = MakeAsyncCommand(ExecuteGet);
        PostCommand = MakeAsyncCommand(ExecutePost);
        Get2Command = MakeAsyncCommand(ExecuteGet2);
        Post2Command = MakeAsyncCommand(ExecutePost2);
    }

    private async ValueTask Execute(string name, Func<HttpClient, int, ValueTask<IRestResponse>> func)
    {
        using var client = httpClientFactory.CreateClient(name);
        var success = 0;

        var watch = StopwatchSlim.StartNew();
        for (var i = 0; i < 100; i++)
        {
            var result = await func(client, i);
            if (result.IsSuccess())
            {
                success++;
            }
        }

        await Application.Current!.MainPage!.DisplayAlert(
            "Result",
            $"Elapsed=[{watch.Elapsed}]\nSuccess=[{success}]\n",
            "ok");
    }

    private async Task ExecuteGet() =>
        await Execute("Default", async (c, i) => await c.GetAsync<ItemResponse>($"/Example/Item?code={i}"));

    private async Task ExecutePost() =>
        await Execute("Default", async (c, _) => await c.PostAsync<EchoResponse>("/Example/Echo", new EchoRequest { Message = "Hello" }));

    private async Task ExecuteGet2() =>
        await Execute("NoBase", async (c, i) => await c.GetAsync<ItemResponse>($"http://192.168.100.9:5000/Example/Item?code={i}"));

    private async Task ExecutePost2() =>
        await Execute("NoBase", async (c, _) => await c.PostAsync<EchoResponse>("http://192.168.100.9:5000/Example/Echo", new EchoRequest { Message = "Hello" }));
}
