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
        EntryHandler.Mapper.Add("On", (handler, _) =>
        {
            var element = (Entry)handler.VirtualView;
            var on = GetOn(element);
            UpdateBehaviors(element, on);
        });
        EditorHandler.Mapper.Add("On", (handler, _) =>
        {
            var element = (Editor)handler.VirtualView;
            var on = GetOn(element);
            UpdateBehaviors(element, on);
        });
    }

    private static void UpdateBehaviors(VisualElement element, bool on)
    {
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
}
