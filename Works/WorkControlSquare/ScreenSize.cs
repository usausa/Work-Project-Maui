namespace WorkControlSquare;

public static class ScreenSize
{
    public static double Width { get; } = DeviceDisplay.Current.MainDisplayInfo.Width / DeviceDisplay.Current.MainDisplayInfo.Density;
}
