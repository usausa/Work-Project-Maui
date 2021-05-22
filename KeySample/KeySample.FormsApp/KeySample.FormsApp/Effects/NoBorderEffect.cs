namespace KeySample.FormsApp.Effects
{
    using System.Linq;
    using Xamarin.Forms;

    public sealed class NoBorderEffect : RoutingEffect
    {
        public static readonly BindableProperty OnProperty = BindableProperty.CreateAttached(
            "On",
            typeof(bool),
            typeof(NoBorderEffect),
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
                element.Effects.Add(new NoBorderEffect());
            }
            else
            {
                var effect = element.Effects.FirstOrDefault(x => x is NoBorderEffect);
                if (effect != null)
                {
                    element.Effects.Remove(effect);
                }
            }
        }

        public NoBorderEffect()
            : base("KeySample.NoBorderEffect")
        {
        }
    }
}
