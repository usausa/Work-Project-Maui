namespace WorkEffect.Behaviors;

public static class NoBorder
{
    public static readonly BindableProperty OnProperty = BindableProperty.CreateAttached(
        "On",
        typeof(bool),
        typeof(NoBorder),
        false,
        propertyChanged: OnOnChanged);

    public static bool GetOn(BindableObject bindable) => (bool)bindable.GetValue(OnProperty);

    public static void SetOn(BindableObject bindable, bool value) => bindable.SetValue(OnProperty, value);

    private static void OnOnChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is not Entry entry)
        {
            return;
        }

        if ((bool)newValue)
        {
            entry.Behaviors.Add(new NoBorderBehavior());
        }
        else
        {
            var behavior = entry.Behaviors.FirstOrDefault(x => x is NoBorderBehavior);
            if (behavior != null)
            {
                entry.Behaviors.Remove(behavior);
            }
        }
    }
}
