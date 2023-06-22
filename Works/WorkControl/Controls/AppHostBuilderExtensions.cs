using WorkControl.Controls.Handlers;

namespace WorkControl.Controls;

internal static class AppHostBuilderExtensions
{
    public static MauiAppBuilder ConfigureCustomControls(this MauiAppBuilder builder)
    {
        return builder
            .ConfigureMauiHandlers(handlers =>
            {
                handlers.AddHandler<SimpleView, SimpleViewHandler>();
            });
    }
}
