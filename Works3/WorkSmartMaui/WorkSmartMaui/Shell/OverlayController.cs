namespace WorkSmartMaui.Shell;

// ReSharper disable MemberCanBeMadeStatic.Global
#pragma warning disable CA1822
public sealed class OverlayController
{
    public static OverlayController Instance { get; } = new();

    public void Clear()
    {
        OverlayView.Instance.UpdateStrategy(DefaultOverlayStrategy.Instance);
    }

    public void Circle()
    {
        OverlayView.Instance.UpdateStrategy(CircleOverlayStrategy.Instance);
    }

    // TODO Progress : return IProgress
}
#pragma warning restore CA1822
