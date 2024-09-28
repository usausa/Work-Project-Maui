namespace WorkRootObject;

public static class RootBind
{
    public static readonly BindableProperty TagProperty = BindableProperty.CreateAttached(
        "Tag",
        typeof(string),
        typeof(RootBind),
        null);

    public static string GetTag(BindableObject bindable) => (string)bindable.GetValue(TagProperty);

    public static void SetTag(BindableObject bindable, string value) => bindable.SetValue(TagProperty, value);
}
