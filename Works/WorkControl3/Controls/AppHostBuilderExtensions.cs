namespace WorkControl3.Controls;

using WorkControl3.Controls.Handlers;

internal static class AppHostBuilderExtensions
{
    public static MauiAppBuilder ConfigureCustomControls(this MauiAppBuilder builder)
    {
        return builder
            .ConfigureMauiHandlers(handlers =>
            {
                handlers.AddHandler<CustomEntry2, CustomEntry2Handler>();
            });
    }
}
