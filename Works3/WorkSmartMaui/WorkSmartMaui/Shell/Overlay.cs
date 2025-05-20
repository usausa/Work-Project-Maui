namespace WorkSmartMaui.Shell;

public static class Overlay
{
    public static readonly BindableProperty VisibleProperty = BindableProperty.CreateAttached(
        "Visible",
        typeof(bool),
        typeof(Overlay),
        false,
        propertyChanged: HandleVisibleChanged);

    public static bool GetVisible(BindableObject obj) =>
        (bool)obj.GetValue(VisibleProperty);

    public static void SetVisible(BindableObject obj, bool value) =>
        obj.SetValue(VisibleProperty, value);

    private static void HandleVisibleChanged(BindableObject bindable, object? oldValue, object? newValue)
    {
        if (oldValue == newValue)
        {
            return;
        }

        if (newValue is true)
        {
            OverlayView.Instance.Show();
        }
        else
        {
            OverlayView.Instance.Hide();
        }
    }
}
