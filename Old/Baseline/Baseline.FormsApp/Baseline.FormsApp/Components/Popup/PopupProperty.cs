namespace Baseline.FormsApp.Components.Popup
{
    using Xamarin.Forms;

    public static class PopupProperty
    {
        public static readonly BindableProperty ThicknessProperty = BindableProperty.Create(
            "Thickness",
            typeof(Thickness),
            typeof(PopupProperty),
            default(Thickness));

        public static Thickness GetThickness(BindableObject view)
        {
            return (Thickness)view.GetValue(ThicknessProperty);
        }

        public static void SetThickness(BindableObject view, Thickness value)
        {
            view.SetValue(ThicknessProperty, value);
        }
    }
}
