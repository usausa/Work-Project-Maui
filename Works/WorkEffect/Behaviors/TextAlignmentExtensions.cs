namespace WorkEffect.Behaviors;

using Android.Views;

public static class TextAlignmentExtensions
{
    public static GravityFlags ToHorizontalGravity(this Microsoft.Maui.TextAlignment alignment)
    {
        return alignment switch
        {
            Microsoft.Maui.TextAlignment.Center => GravityFlags.CenterHorizontal,
            Microsoft.Maui.TextAlignment.End => GravityFlags.Right,
            Microsoft.Maui.TextAlignment.Start => GravityFlags.Left,
            _ => throw new InvalidOperationException(alignment.ToString())
        };
    }

    public static GravityFlags ToVerticalGravity(this Microsoft.Maui.TextAlignment alignment)
    {
        return alignment switch
        {
            Microsoft.Maui.TextAlignment.Center => GravityFlags.CenterVertical,
            Microsoft.Maui.TextAlignment.End => GravityFlags.Bottom,
            Microsoft.Maui.TextAlignment.Start => GravityFlags.Top,
            _ => throw new InvalidOperationException(alignment.ToString())
        };
    }
}
