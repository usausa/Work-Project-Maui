namespace WorkCustomMove.Effects
{
    using System.Linq;

    using Xamarin.Forms;

    public sealed class DisableKeyboardOnFocusEffect : RoutingEffect
    {
        public static readonly BindableProperty OnProperty = BindableProperty.CreateAttached(
            "On",
            typeof(bool),
            typeof(DisableKeyboardOnFocusEffect),
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
            if (bindable is not Element element)
            {
                return;
            }

            if ((bool)newValue)
            {
                element.Effects.Add(new DisableKeyboardOnFocusEffect());
            }
            else
            {
                var effect = element.Effects.FirstOrDefault(x => x is DisableKeyboardOnFocusEffect);
                if (effect != null)
                {
                    element.Effects.Remove(effect);
                }
            }
        }

        public DisableKeyboardOnFocusEffect()
            : base("WorkCustomMove.DisableKeyboardOnFocusEffect")
        {
        }
    }
}
