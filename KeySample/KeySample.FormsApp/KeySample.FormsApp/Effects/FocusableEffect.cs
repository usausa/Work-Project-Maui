namespace KeySample.FormsApp.Effects
{
    using System.Linq;
    using Xamarin.Forms;

    public sealed class FocusableEffect : RoutingEffect
    {
        public static readonly BindableProperty AttachProperty = BindableProperty.CreateAttached(
            "Attach",
            typeof(bool),
            typeof(FocusableEffect),
            false,
            propertyChanged: OnAttachChanged);

        public static bool GetAttach(BindableObject view)
        {
            return (bool)view.GetValue(AttachProperty);
        }

        public static void SetAttach(BindableObject view, bool value)
        {
            view.SetValue(AttachProperty, value);
        }

        private static void OnAttachChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is not Button button)
            {
                return;
            }

            if ((bool)newValue)
            {
                button.Effects.Add(new FocusableEffect());
            }
            else
            {
                var effect = button.Effects.FirstOrDefault(x => x is FocusableEffect);
                if (effect != null)
                {
                    button.Effects.Remove(effect);
                }
            }
        }

        public FocusableEffect()
            : base("KeySample.FocusEffect")
        {
        }
    }
}
