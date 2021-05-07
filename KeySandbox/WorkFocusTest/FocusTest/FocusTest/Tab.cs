using Xamarin.Forms;

namespace FocusTest
{
    public static class Tab
    {
        public static readonly BindableProperty IndexProperty = BindableProperty.CreateAttached(
            "Index",
            typeof(int),
            typeof(Tab),
            0);

        public static int GetIndex(BindableObject view)
        {
            return (int)view.GetValue(IndexProperty);
        }

        public static void SetIndex(BindableObject view, int value)
        {
            view.SetValue(IndexProperty, value);
        }
    }
}
