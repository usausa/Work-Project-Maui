namespace WorkDesign;

public partial class MiscPage : ContentPage
{
	public MiscPage()
	{
		InitializeComponent();
	}
}

public static class ScreenSize
{
	public static double Width1 => 1 / DeviceDisplay.MainDisplayInfo.Density;

    public static double Width2 => 2 / DeviceDisplay.MainDisplayInfo.Density;

    public static double Width3 => 3 / DeviceDisplay.MainDisplayInfo.Density;

    public static double Width4 => 4 / DeviceDisplay.MainDisplayInfo.Density;

    public static double Width5 => 5 / DeviceDisplay.MainDisplayInfo.Density;

    public static double Width6 => 6 / DeviceDisplay.MainDisplayInfo.Density;

    public static double Width8 => 8 / DeviceDisplay.MainDisplayInfo.Density;

    public static double Width10 => 10 / DeviceDisplay.MainDisplayInfo.Density;

    public static double Width12 => 12 / DeviceDisplay.MainDisplayInfo.Density;

    public static double Width16 => 16 / DeviceDisplay.MainDisplayInfo.Density;
}