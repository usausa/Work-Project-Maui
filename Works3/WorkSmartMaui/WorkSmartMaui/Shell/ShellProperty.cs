namespace WorkSmartMaui.Shell;

public static class ShellProperty
{
    public static readonly BindableProperty ProgressVisibleProperty = BindableProperty.CreateAttached(
        "ProgressVisible",
        typeof(bool),
        typeof(ShellProperty),
        false,
        propertyChanged: HandleProgressVisibleChanged);

    public static readonly BindableProperty ProgressViewProperty = BindableProperty.CreateAttached(
        "ProgressView",
        typeof(IProgressView),
        typeof(ShellProperty),
        null,
        propertyChanged: HandleProgressViewChanged);

    public static bool GetProgressVisible(BindableObject obj) =>
        (bool)obj.GetValue(ProgressVisibleProperty);

    public static void SetProgressVisible(BindableObject obj, bool value) =>
        obj.SetValue(ProgressVisibleProperty, value);

    public static IProgressView? GetProgressView(BindableObject obj) =>
        (IProgressView?)obj.GetValue(ProgressViewProperty);

    public static void SetProgressView(BindableObject obj, IProgressView? value) =>
        obj.SetValue(ProgressViewProperty, value);

    private static void HandleProgressVisibleChanged(BindableObject bindable, object? oldValue, object? newValue)
    {
        if (oldValue == newValue)
        {
            return;
        }

        var view = GetProgressView(bindable);
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

    private static void HandleProgressViewChanged(BindableObject bindable, object? oldValue, object? newValue)
    {
        if (oldValue == newValue)
        {
            return;
        }

        if (oldValue is IProgressView oldProgressView)
        {
            oldProgressView.Hide();
        }
        if (newValue is IProgressView newProgressView)
        {
            var visible = GetProgressVisible(bindable);
            if (visible)
            {
                newProgressView.Show();
            }
            else
            {
                newProgressView.Hide();
            }
        }
    }
}
