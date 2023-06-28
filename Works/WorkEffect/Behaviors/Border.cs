using Microsoft.Maui.Handlers;

namespace WorkEffect.Behaviors;

public static class Border
{
    public static readonly BindableProperty BorderWidthProperty =
        BindableProperty.CreateAttached(
            "BorderWidth",
            typeof(double),
            typeof(Border),
            default(double));

    public static readonly BindableProperty BorderColorProperty =
        BindableProperty.CreateAttached(
            "BorderColor",
            typeof(Color),
            typeof(Border),
            Colors.Transparent);

    public static void SetBorderWidth(BindableObject bindable, double value) => bindable.SetValue(BorderWidthProperty, value);

    public static double GetBorderWidth(BindableObject bindable) => (double)bindable.GetValue(BorderWidthProperty);

    public static void SetBorderColor(BindableObject bindable, Color value) => bindable.SetValue(BorderColorProperty, value);

    public static Color GetBorderColor(BindableObject bindable) => (Color)bindable.GetValue(BorderColorProperty);

    public static void UseCustomMapper()
    {
#if ANDROID
        EntryHandler.Mapper.Add("BorderWidth", static (handler, _) => UpdateBehaviors((Entry)handler.VirtualView));
        EntryHandler.Mapper.Add("BorderColor", static (handler, _) => UpdateBehaviors((Entry)handler.VirtualView));
        LabelHandler.Mapper.Add("BorderWidth", static (handler, _) => UpdateBehaviors((Label)handler.VirtualView));
        LabelHandler.Mapper.Add("BorderColor", static (handler, _) => UpdateBehaviors((Label)handler.VirtualView));
#endif
    }

#if ANDROID
    private static void UpdateBehaviors(VisualElement element)
    {
        var width = GetBorderWidth(element);
        var on = width > 0;
        var behavior = element.Behaviors.OfType<BorderBehavior>().FirstOrDefault();
        if (on)
        {
            if (behavior is not null)
            {
                behavior.UpdateBorder();
            }
            else
            {
                element.Behaviors.Add(new BorderBehavior());
            }
        }
        else if (behavior is not null)
        {
            element.Behaviors.Remove(behavior);
        }
    }
#endif
}
