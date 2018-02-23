using System;
using Xamarin.Forms;

namespace BindingTest
{
    public class ViewProperty
    {
        public static readonly BindableProperty TitleProperty = BindableProperty.CreateAttached(
            "Title",
            typeof(string),
            typeof(ViewProperty),
            null,
            propertyChanged: PropertyChanged);

        public static string GetTitle(BindableObject view)
        {
            System.Diagnostics.Debug.WriteLine("GetTitle " + view.GetHashCode());

            return (string)view.GetValue(TitleProperty);
        }

        public static void SetTitle(BindableObject view, string value)
        {
            System.Diagnostics.Debug.WriteLine("SetTitle " + view.GetHashCode());

            view.SetValue(TitleProperty, value);
        }

        private static void PropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            System.Diagnostics.Debug.WriteLine("PropertyChanged " + oldValue + " " + newValue);
        }
    }
}
