namespace Template.MobileApp.Behaviors;

public static partial class EntryOption
{
    // ReSharper disable InconsistentNaming
    public static readonly BindableProperty NoBorderProperty = BindableProperty.CreateAttached(
        "NoBorder",
        typeof(bool),
        typeof(EntryOption),
        false);
    // ReSharper restore InconsistentNaming

    public static bool GetNoBorder(BindableObject bindable) => (bool)bindable.GetValue(NoBorderProperty);

    public static void SetNoBorder(BindableObject bindable, bool value) => bindable.SetValue(NoBorderProperty, value);
}
