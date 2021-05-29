namespace KeySample.FormsApp.Input
{
    using Xamarin.Forms;

    public static class Shortcut
    {
        public static readonly BindableProperty KeyProperty = BindableProperty.CreateAttached(
            "Key",
            typeof(KeyCodes),
            typeof(Shortcut),
            null);

        public static KeyCodes GetKey(BindableObject view)
        {
            return (KeyCodes)view.GetValue(KeyProperty);
        }

        public static void SetKey(BindableObject view, KeyCodes value)
        {
            view.SetValue(KeyProperty, value);
        }
    }
}
