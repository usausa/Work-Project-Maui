using System.Diagnostics;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using Smart.Maui.Interactivity;
using Smart.Maui.ViewModels;

using System.Windows.Input;

namespace WorkHybridWeb;


[JsonSourceGenerationOptions(WriteIndented = true)]
[JsonSerializable(typeof(int))]
internal partial class HybridSampleJsContext : JsonSerializerContext
{
}

internal class WebBasicPageViewModel : ExtendViewModelBase
{
    public WebViewController<WebBasicPageViewModel> Controller { get; }

    public ICommand MessageCommand { get; }

    public ICommand AddCommand { get; }

    public ICommand BackCommand { get; }

    public WebBasicPageViewModel()
    {
        Controller = new WebViewController<WebBasicPageViewModel>(this);

        Controller.RawMessageReceived += (_, args) =>
        {
            Debug.WriteLine(args.Message);
        };

        MessageCommand = MakeDelegateCommand(() => Controller.SendRawMessage("Hello from C#!"));
        AddCommand = MakeAsyncCommand(async () =>
        {
            var result = await Controller.InvokeJavaScriptAsync(
                "Add",
                HybridSampleJsContext.Default.Int32,
                [1, 2],
                [HybridSampleJsContext.Default.Int32, HybridSampleJsContext.Default.Int32]);
            Debug.WriteLine(result);
        });
        BackCommand = MakeDelegateCommand(() => Controller.GoBack());
    }

    public int Calc(int x, int y) => x + y;

    public async Task<DataEntity> ExecuteAsync(int id, string name)
    {
        await Task.Delay(1000);
        return new DataEntity { Id = id, Name = name };
    }
}

public sealed class DataEntity
{
    public long Id { get; set; }

    public string Name { get; set; } = default!;
}

public static class WebViewBind
{
    public static readonly BindableProperty ControllerProperty = BindableProperty.CreateAttached(
        "Controller",
        typeof(IWebViewController),
        typeof(WebViewBind),
        null,
        propertyChanged: BindChanged);

    public static IWebViewController? GetController(BindableObject bindable) =>
        (IWebViewController)bindable.GetValue(ControllerProperty);

    public static void SetController(BindableObject bindable, IWebViewController? value) =>
        bindable.SetValue(ControllerProperty, value);

    private static void BindChanged(BindableObject bindable, object? oldValue, object? newValue)
    {
        if (bindable is not HybridWebView view)
        {
            return;
        }

        if (oldValue is not null)
        {
            var behavior = view.Behaviors.FirstOrDefault(static x => x is WebViewBindBehavior);
            if (behavior is not null)
            {
                view.Behaviors.Remove(behavior);
            }
        }

        if (newValue is not null)
        {
            view.Behaviors.Add(new WebViewBindBehavior());
        }
    }

    private sealed class WebViewBindBehavior : BehaviorBase<HybridWebView>
    {
        private IWebViewController? controller;

        protected override void OnAttachedTo(HybridWebView bindable)
        {
            base.OnAttachedTo(bindable);

            controller = GetController(bindable);
            controller?.Attach(bindable);
        }

        protected override void OnDetachingFrom(HybridWebView bindable)
        {
            controller?.Detach();
            controller = null;

            base.OnDetachingFrom(bindable);
        }
    }
}


public interface IWebViewController
{
    void Attach(HybridWebView view);

    void Detach();

    void RaiseRawMessageReceived(object sender, HybridWebViewRawMessageReceivedEventArgs e);
}

public abstract class WebViewControllerBase : IWebViewController
{
    public event EventHandler<HybridWebViewRawMessageReceivedEventArgs>? RawMessageReceived;

    private HybridWebView? webView;

    void IWebViewController.Attach(HybridWebView view)
    {
        webView = view;
        Attached(view);
    }

    protected abstract void Attached(HybridWebView view);

    void IWebViewController.Detach()
    {
        webView = null;
    }

    void IWebViewController.RaiseRawMessageReceived(object sender, HybridWebViewRawMessageReceivedEventArgs e)
    {
        RawMessageReceived?.Invoke(sender, e);
    }

    public void SendRawMessage(string rawMessage)
    {
        webView?.SendRawMessage(rawMessage);
    }

    public async Task<TReturnType?> InvokeJavaScriptAsync<TReturnType>(
        string methodName,
        JsonTypeInfo<TReturnType> returnTypeJsonTypeInfo,
        object?[]? paramValues = null,
        JsonTypeInfo?[]? paramJsonTypeInfos = null)
    {
        if (webView is not null)
        {
            return await webView.InvokeJavaScriptAsync(methodName, returnTypeJsonTypeInfo, paramValues,
                paramJsonTypeInfos);
        }

        return default;
    }

    public async Task<string?> EvaluateJavaScriptAsync(string script)
    {
        if (webView is not null)
        {
            return await webView.EvaluateJavaScriptAsync(script);
        }

        return null;
    }

    public void GoBack()
    {
#if ANDROID
        if (webView?.Handler?.PlatformView is Android.Webkit.WebView view)
        {
            if (view.CanGoBack())
            {
                view.GoBack();
            }
        }
#endif
#if WINDOWS
        if (webView?.Handler?.PlatformView is Microsoft.UI.Xaml.Controls.WebView2 { CanGoBack: true } view)
        {
            view.GoBack();
        }
#endif
    }
}

public sealed class WebViewController : WebViewControllerBase
{
    protected override void Attached(HybridWebView view)
    {
    }
}

public sealed class WebViewController<T> : WebViewControllerBase
        where T : class
{
    private readonly T target;

    public WebViewController(T target)
    {
        this.target = target;
    }

    protected override void Attached(HybridWebView view)
    {
        view.SetInvokeJavaScriptTarget(target);
    }
}