namespace Template.MobileApp.Behaviors;

using Android.Widget;

using Microsoft.Maui.Handlers;

public static class EntryOption
{
    // ReSharper disable InconsistentNaming
    public static readonly BindableProperty SelectAllOnFocusProperty = BindableProperty.CreateAttached(
        "SelectAllOnFocus",
        typeof(bool),
        typeof(EntryOption),
        false);
    // ReSharper restore InconsistentNaming

    public static bool GetSelectAllOnFocus(BindableObject bindable) => (bool)bindable.GetValue(SelectAllOnFocusProperty);

    public static void SetSelectAllOnFocus(BindableObject bindable, bool value) => bindable.SetValue(SelectAllOnFocusProperty, value);

    public static void UseCustomMapper()
    {
#if ANDROID
        EntryHandler.Mapper.Add("SelectAllOnFocus", static (handler, _) => UpdateBehaviors(handler.PlatformView, (Entry)handler.VirtualView));
        EditorHandler.Mapper.Add("SelectAllOnFocus", static (handler, _) => UpdateBehaviors(handler.PlatformView, (Editor)handler.VirtualView));
#endif
    }

#if ANDROID
    private static void UpdateBehaviors(EditText editText, VisualElement element)
    {
        var value = GetSelectAllOnFocus(element);
        editText.SetSelectAllOnFocus(value);
    }
#endif
}
