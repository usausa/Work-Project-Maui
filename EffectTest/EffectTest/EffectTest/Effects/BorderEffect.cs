namespace EffectTest.Effects
{
    using System.Linq;

    using Xamarin.Forms;

    public static class Border
    {
        public static readonly BindableProperty PaddingProperty =
            BindableProperty.CreateAttached(
                "Padding",
                typeof(Thickness),
                typeof(Border),
                default(Thickness),
                propertyChanged: OnPropertyChanged);

        public static readonly BindableProperty WidthProperty =
            BindableProperty.CreateAttached(
                "Width",
                typeof(double),
                typeof(Border),
                default(double),
                propertyChanged: OnPropertyChanged);

        public static readonly BindableProperty ColorProperty =
            BindableProperty.CreateAttached(
                "Color",
                typeof(Color),
                typeof(Border),
                Color.Transparent);

        public static readonly BindableProperty RadiusProperty =
            BindableProperty.CreateAttached(
                "Radius",
                typeof(double),
                typeof(Border),
                default(double),
                propertyChanged: OnPropertyChanged);

        public static void SetPadding(BindableObject view, Thickness value)
        {
            view.SetValue(PaddingProperty, value);
        }

        public static Thickness GetPadding(BindableObject view)
        {
            return (Thickness)view.GetValue(PaddingProperty);
        }

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

        private static void OnPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (!(bindable is VisualElement element))
            {
                return;
            }

            var width = GetWidth(bindable);
            var radius = GetRadius(bindable);
            var padding = GetPadding(bindable);
            var on = (width > 0) || (radius > 0) || (padding.Left > 0) || (padding.Top > 0) || (padding.Right > 0) || (padding.Bottom > 0);

            var effect = element.Effects.OfType<BorderEffect>().FirstOrDefault();
            if (on && effect == null)
            {
                element.Effects.Add(new BorderEffect());
            }
            else if (!on && effect != null)
            {
                element.Effects.Remove(effect);
            }
        }
    }

    public sealed class BorderEffect : RoutingEffect
    {
        public BorderEffect()
            : base("Test.BorderEffect")
        {
        }
    }
}
