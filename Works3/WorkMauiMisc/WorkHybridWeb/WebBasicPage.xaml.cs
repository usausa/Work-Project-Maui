using System.Diagnostics;

namespace WorkHybridWeb;

using Microsoft.Maui;
using Microsoft.Maui.Controls;

using System.Text.Json.Serialization;

[JsonSourceGenerationOptions(WriteIndented = true)]
[JsonSerializable(typeof(int))]
//[JsonSerializable(typeof(Dictionary<string, string>))]
//[JsonSerializable(typeof(string))]
internal partial class HybridSampleJsContext : JsonSerializerContext
{
    // This type's attributes specify JSON serialization info to preserve type structure
    // for trimmed builds.
}

public partial class WebBasicPage : ContentPage
{
	public WebBasicPage()
	{
		InitializeComponent();

        HybridWebView.SetInvokeJavaScriptTarget(this);
    }

    private void OnRawMessageClicked(object? sender, EventArgs e)
    {
        HybridWebView.SendRawMessage("Hello from C#!");
    }

    private async void OnJavascriptClicked(object? sender, EventArgs e)
    {
        var x = 123;
        var y = 321;

        // TODO Exception ?
        try
        {
            var result = await HybridWebView.InvokeJavaScriptAsync<int>(
                "AddNumbers", // JavaScript method name
                HybridSampleJsContext.Default.Int32, // JSON serialization info for return type
                [x, y], // Parameter Int32
                [HybridSampleJsContext.Default.Int32, HybridSampleJsContext.Default.Int32]); // JSON serialization info for each parameter
            Debug.WriteLine(result);
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
        }
    }

    private async void OnJavascriptAsyncClicked(object? sender, EventArgs e)
    {
        //try
        //{
        //    Dictionary<string, string>? asyncResult = await HybridWebView.InvokeJavaScriptAsync<Dictionary<string, string>>(
        //        "EvaluateMeWithParamsAndAsyncReturn", // JavaScript method name
        //        HybridSampleJsContext.Default.DictionaryStringString, // JSON serialization info for return type
        //        ["new_key", "new_value"], // Parameter values
        //        [HybridSampleJsContext.Default.String, HybridSampleJsContext.Default.String]); // JSON serialization info for each parameter
        //}
        //catch (Exception ex)
        //{
        //    Debug.WriteLine(ex);
        //}
    }

    private async void OnHybridWebViewRawMessageReceived(object sender, HybridWebViewRawMessageReceivedEventArgs e)
    {
        await DisplayAlert("Raw Message Received", e.Message, "OK");
    }

    public void DoSyncWork()
    {
        Debug.WriteLine("DoSyncWork");
    }

    public void DoSyncWorkParams(int i, string s)
    {
        Debug.WriteLine($"DoSyncWorkParams: {i}, {s}");
    }

    public string DoSyncWorkReturn()
    {
        Debug.WriteLine("DoSyncWorkReturn");
        return "Hello from C#!";
    }

    public SyncReturn DoSyncWorkParamsReturn(int i, string s)
    {
        Debug.WriteLine($"DoSyncWorkParamReturn: {i}, {s}");
        return new SyncReturn
        {
            Message = "Hello from C#!" + s,
            Value = i
        };
    }

    public async Task DoAsyncWork()
    {
        Debug.WriteLine("DoAsyncWork");
        await Task.Delay(1000);
    }

    public async Task DoAsyncWorkParams(int i, string s)
    {
        Debug.WriteLine($"DoAsyncWorkParams: {i}, {s}");
        await Task.Delay(1000);
    }

    public async Task<String> DoAsyncWorkReturn()
    {
        Debug.WriteLine("DoAsyncWorkReturn");
        await Task.Delay(1000);
        return "Hello from C#!";
    }

    public async Task<SyncReturn> DoAsyncWorkParamsReturn(int i, string s)
    {
        Debug.WriteLine($"DoAsyncWorkParamsReturn: {i}, {s}");
        await Task.Delay(1000);
        return new SyncReturn
        {
            Message = "Hello from C#!" + s,
            Value = i
        };
    }

    public class SyncReturn
    {
        public string? Message { get; set; }
        public int Value { get; set; }
    }
}
