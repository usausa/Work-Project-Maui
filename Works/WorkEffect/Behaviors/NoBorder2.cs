using Microsoft.Maui.Handlers;

namespace WorkEffect.Behaviors;

public static class NoBorder2
{
    public static readonly BindableProperty OnProperty = BindableProperty.CreateAttached(
        "On",
        typeof(bool),
        typeof(NoBorder2),
        false);

    public static bool GetOn(BindableObject bindable) => (bool)bindable.GetValue(OnProperty);

    public static void SetOn(BindableObject bindable, bool value) => bindable.SetValue(OnProperty, value);

    public static void UseCustomMapper()
    {
#if ANDROID
        EntryHandler.Mapper.Add("On", static (handler, _) => UpdateBehaviors((Entry)handler.VirtualView));
        EditorHandler.Mapper.Add("On", static (handler, _) => UpdateBehaviors((Editor)handler.VirtualView));
#endif
    }

#if ANDROID
    private static void UpdateBehaviors(VisualElement element)
    {
        var on = GetOn(element);
        if (on)
        {
            element.Behaviors.Add(new NoBorderBehavior());
        }
        else
        {
            var behavior = element.Behaviors.FirstOrDefault(x => x is NoBorderBehavior);
            if (behavior != null)
            {
                element.Behaviors.Remove(behavior);
            }
        }
    }
#endif
}
