namespace EffectTest.Effects
{
    using System.Linq;

    using Xamarin.Forms;

    public sealed class BorderEffect : RoutingEffect
    {
        public static readonly BindableProperty WidthProperty =
            BindableProperty.CreateAttached(
                "Width",
                typeof(double),
                typeof(BorderEffect),
                default(double),
                propertyChanged: OnWidthPropertyChanged);

        public static readonly BindableProperty ColorProperty =
            BindableProperty.CreateAttached(
                "Color",
                typeof(Color),
                typeof(BorderEffect),
                Color.Transparent);

        public static readonly BindableProperty RadiusProperty =
            BindableProperty.CreateAttached(
                "Radius",
                typeof(double),
                typeof(BorderEffect),
                default(double));

        public static void SetWidth(BindableObject view, double value)
        {
            view.SetValue(WidthProperty, value);
        }

        public static double GetWidth(BindableObject view)
        {
            return (double)view.GetValue(WidthProperty);
        }

        public static void SetColor(BindableObject view, Color value)
        {
            view.SetValue(ColorProperty, value);
        }

        public static Color GetColor(BindableObject view)
        {
            return (Color)view.GetValue(ColorProperty);
        }

        public static void SetRadius(BindableObject view, double value)
        {
            view.SetValue(RadiusProperty, value);
        }

        public static double GetRadius(BindableObject view)
        {
            return (double)view.GetValue(RadiusProperty);
        }

        private static void OnWidthPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (!(bindable is VisualElement element))
            {
                return;
            }

            var effect = element.Effects.OfType<BorderEffect>().FirstOrDefault();
            if (effect != null)
            {
                element.Effects.Remove(effect);
            }

            var width = (double)newValue;
            if (width > 0)
            {
                element.Effects.Add(new BorderEffect());
            }
        }

        public BorderEffect()
            : base("Test.BorderEffect")
        {
        }
    }
}
