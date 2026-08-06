namespace Template.MobileApp;

using Microsoft.Extensions.DependencyInjection;

using Smart.Mvvm.Resolver;

using Template.MobileApp.Services;

public sealed class ApplicationInitializer : IMauiInitializeService
{
    // 非同期初期化のTask。App.OnStartが画面遷移前に完了を待つ
    public Task StartupTask { get; private set; } = Task.CompletedTask;

    public void Initialize(IServiceProvider services)
    {
        // Setup provider
        ResolveProvider.Default.Provider = services;

        var settings = services.GetRequiredService<Settings>();

        // Initial setting
        if (String.IsNullOrEmpty(settings.ApiEndPoint) && !String.IsNullOrEmpty(EmbeddedProperty.ApiEndPoint))
        {
            settings.ApiEndPoint = EmbeddedProperty.ApiEndPoint;
        }

        // Setup navigator
        var navigator = services.GetRequiredService<INavigator>();
        navigator.Navigated += (_, args) =>
        {
            // for debug
            System.Diagnostics.Debug.WriteLine(
                $"Navigated: [{args.Context.FromId}]->[{args.Context.ToId}] : stacked=[{navigator.StackedCount}]");
        };

        // Setting
        if (String.IsNullOrEmpty(settings.UniqueId))
        {
            var uniqueId = Guid.NewGuid();
            settings.UniqueId = uniqueId.ToString();
        }

        var apiContext = services.GetRequiredService<ApiContext>();
        if (!String.IsNullOrEmpty(settings.ApiEndPoint))
        {
            apiContext.BaseAddress = new Uri(settings.ApiEndPoint);
        }

        // Service
        StartupTask = InitializeAsync(services);
    }

    private static async Task InitializeAsync(IServiceProvider services)
    {
        var dataService = services.GetRequiredService<DataService>();
        await dataService.RebuildAsync();
    }
}
