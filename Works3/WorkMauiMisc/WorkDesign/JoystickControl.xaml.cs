using System.Diagnostics;

namespace WorkDesign;

public partial class JoystickControl : ContentView
{
    private double _radius = 40;

    public JoystickControl()
	{
		InitializeComponent();

        var panGesture = new PanGestureRecognizer();
        panGesture.PanUpdated += OnPanUpdated;
        this.GestureRecognizers.Add(panGesture);
    }

    private void OnPanUpdated(object sender, PanUpdatedEventArgs e)
    {
        switch (e.StatusType)
        {
            case GestureStatus.Running:
                double x = Math.Clamp(e.TotalX, -_radius, _radius);
                double y = Math.Clamp(e.TotalY, -_radius, _radius);
                Thumb.TranslationX = x;
                Thumb.TranslationY = y;
                break;

            case GestureStatus.Completed:
                var direction = DirectionHelper.GetDirection(Thumb.TranslationX, Thumb.TranslationY);
                Debug.WriteLine(direction);
                Thumb.TranslationX = 0;
                Thumb.TranslationY = 0;
                break;
        }
    }
}

public enum StickDirection
{
    None,
    Up,
    Down,
    Left,
    Right
}

public static class DirectionHelper
{
    private static double sensitivity = 20;

    public static StickDirection GetDirection(double x, double y)
    {
        if (Math.Abs(x) > Math.Abs(y))
            return x > sensitivity ? StickDirection.Right : x < -sensitivity ? StickDirection.Left : StickDirection.None;
        else
            return y > sensitivity ? StickDirection.Down : y < -sensitivity ? StickDirection.Up : StickDirection.None;
    }
}