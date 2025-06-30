using System.Diagnostics;

namespace WorkDebug;

using Microsoft.Maui.Handlers;

public static class Extensions
{
    public static MauiAppBuilder UseDebugOverlay(this MauiAppBuilder builder)
    {
        builder.ConfigureMauiHandlers(_ =>
        {
            WindowHandler.Mapper.AppendToMapping("DebugOverlay", (handler, _) =>
            {
                var overlay = new DebugOverlay(handler.VirtualView);
                handler.VirtualView.AddOverlay(overlay);
            });
        });

        return builder;
    }
}

public sealed class DebugOverlay : WindowOverlay
{
    public DebugOverlay(IWindow window) : base(window)
    {
        Tapped += OnDebugOverlayTapped;
    }

    private void OnDebugOverlayTapped(object? sender, WindowOverlayTappedEventArgs e)
    {
        Debug.WriteLine($"* Tapped. point={e.Point}");
    }
}
