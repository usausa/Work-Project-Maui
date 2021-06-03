using System.Diagnostics;

namespace WorkAttached
{
    using Xamarin.Forms;

    public static class Shortcut
    {
        public static readonly BindableProperty KeyProperty = BindableProperty.CreateAttached(
            "Key",
            typeof(int),
            typeof(Shortcut),
            null,
            propertyChanged: PropertyChanged);

        public static int GetKey(BindableObject view)
        {
            return (int)view.GetValue(KeyProperty);
        }

        public static void SetKey(BindableObject view, int value)
        {
            view.SetValue(KeyProperty, value);
        }

        private static void PropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            Debug.WriteLine($"**** PropertyChanged {bindable.GetType()} {newValue}");
        }
    }
}
