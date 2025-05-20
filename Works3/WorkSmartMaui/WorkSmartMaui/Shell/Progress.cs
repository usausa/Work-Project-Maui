namespace WorkSmartMaui.Shell;

public static class Progress
{
    public static readonly BindableProperty VisibleProperty = BindableProperty.CreateAttached(
        "Visible",
        typeof(bool),
        typeof(Progress),
        false,
        propertyChanged: HandleVisibleChanged);

    public static readonly BindableProperty ViewProperty = BindableProperty.CreateAttached(
        "View",
        typeof(IProgressView),
        typeof(Progress),
        null,
        propertyChanged: HandleViewChanged);

    public static bool GetVisible(BindableObject obj) =>
        (bool)obj.GetValue(VisibleProperty);

    public static void SetVisible(BindableObject obj, bool value) =>
        obj.SetValue(VisibleProperty, value);

    public static IProgressView? GetView(BindableObject obj) =>
        (IProgressView?)obj.GetValue(ViewProperty);

    public static void SetView(BindableObject obj, IProgressView? value) =>
        obj.SetValue(ViewProperty, value);

    private static void HandleVisibleChanged(BindableObject bindable, object? oldValue, object? newValue)
    {
        if (oldValue == newValue)
        {
            return;
        }

        var view = GetView(bindable);
        if (view is null)
        {
            return;
        }

        if (newValue is true)
        {
            view.Show();
        }
        else
        {
            view.Hide();
        }
    }

    private static void HandleViewChanged(BindableObject bindable, object? oldValue, object? newValue)
    {
        if (oldValue == newValue)
        {
            return;
        }

        if (oldValue is IProgressView oldView)
        {
            oldView.Hide();
        }
        if (newValue is IProgressView newView)
        {
            var visible = GetVisible(bindable);
            if (visible)
            {
                newView.Show();
            }
            else
            {
                newView.Hide();
            }
        }
    }
}
