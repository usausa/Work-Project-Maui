using System.Diagnostics;

namespace WorkSelect;

public static class Select
{
    public static readonly BindableProperty ValueProperty = BindableProperty.CreateAttached(
        "Value",
        typeof(object),
        typeof(Select),
        null,
        propertyChanged: HandlePropertyChanged);

    public static readonly BindableProperty EmptyStringProperty = BindableProperty.CreateAttached(
        "EmptyString",
        typeof(string),
        typeof(Select),
        null,
        propertyChanged: HandlePropertyChanged);

    public static object? GetValue(BindableObject obj) => obj.GetValue(ValueProperty);

    public static void SetValue(BindableObject obj, object? value) => obj.SetValue(ValueProperty, value);

    public static string? GetEmptyString(BindableObject obj) => (string?)obj.GetValue(EmptyStringProperty);

    public static void SetEmptyString(BindableObject obj, string? value) => obj.SetValue(EmptyStringProperty, value);

    private static void HandlePropertyChanged(BindableObject bindable, object oldValue, object newValue)
    {
        Debug.WriteLine("** " + newValue);
    }
}
