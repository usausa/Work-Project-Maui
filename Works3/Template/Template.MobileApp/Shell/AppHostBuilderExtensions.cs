namespace Template.MobileApp.Shell;

public static class AppHostBuilderExtensions
{
    public static MauiAppBuilder UseCustomProgress(this MauiAppBuilder builder)
    {
        return builder.UseCustomProgress(_ => { });
    }

    public static MauiAppBuilder UseCustomProgress(this MauiAppBuilder builder, Action<ProgressConfig> action)
    {
        builder.Services.AddSingleton(_ =>
        {
            var config = new ProgressConfig();
            action(config);
            return config;
        });

        builder.Services.AddSingleton<IndicatorProgressStrategy>();
        builder.Services.AddSingleton<MessageProgressStrategy>();
        builder.Services.AddSingleton<RateProgressStrategy>();

        builder.Services.AddSingleton<IProgressController, ProgressController>();

        builder.Services.AddSingleton<ProgressView>();
        builder.Services.AddSingleton<IProgressView>(static p => p.GetRequiredService<ProgressView>());
        builder.Services.AddSingleton<IProgressDrawer>(static p => p.GetRequiredService<ProgressView>());
        builder.Services.AddSingleton<IProgressStrategyUpdate>(static p => p.GetRequiredService<ProgressView>());
        builder.Services.AddSingleton<IProgressStrategyCallback>(static p => p.GetRequiredService<ProgressView>());

        return builder;
    }
}
