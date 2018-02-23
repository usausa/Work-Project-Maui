namespace BindingTest
{
    using Xamarin.Forms;

    public class ViewProperty
    {
        public static readonly BindableProperty SinkProperty = BindableProperty.CreateAttached(
            "Sink",
            typeof(IViewPropertySink),
            typeof(ViewProperty),
            null);

        public static IViewPropertySink GetSink(BindableObject view)
        {
            return (IViewPropertySink)view.GetValue(SinkProperty);
        }

        public static void SetSink(BindableObject view, IViewPropertySink value)
        {
            view.SetValue(SinkProperty, value);
        }

        public static readonly BindableProperty TitleProperty = BindableProperty.CreateAttached(
            "Title",
            typeof(string),
            typeof(ViewProperty),
            null,
            propertyChanged: PropertyChanged);

        public static string GetTitle(BindableObject view)
        {
            return (string)view.GetValue(TitleProperty);
        }

        public static void SetTitle(BindableObject view, string value)
        {
            view.SetValue(TitleProperty, value);
        }

        private static void PropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var parent = ((ContentView)bindable).Parent;
            if (parent == null)
            {
                return;
            }

            var sink = GetSink(parent);
            if (sink != null)
            {
                sink.Title = GetTitle(bindable);
            }
        }
    }
}
