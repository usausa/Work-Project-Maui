namespace WorkEntry
{
    using System.Linq;

    using Xamarin.Forms;

    public static class InputEntry
    {
        public static readonly BindableProperty OnProperty = BindableProperty.CreateAttached(
            "On",
            typeof(bool),
            typeof(InputEntryEffect),
            false,
            propertyChanged: OnChanged);

        public static bool GetOn(BindableObject view)
        {
            return (bool)view.GetValue(OnProperty);
        }

        public static void SetOn(BindableObject view, bool value)
        {
            view.SetValue(OnProperty, value);
        }

        private static void OnChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is not Entry entry)
            {
                return;
            }

            if ((bool)newValue)
            {
                entry.Effects.Add(new InputEntryEffect());
            }
            else
            {
                var effect = entry.Effects.FirstOrDefault(x => x is InputEntryEffect);
                if (effect != null)
                {
                    entry.Effects.Remove(effect);
                }
            }
        }
    }

    public sealed class InputEntryEffect : RoutingEffect
    {
        public InputEntryEffect()
            : base("WorkEntry.InputEntryEffect")
        {
        }
    }
}
