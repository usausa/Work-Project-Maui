namespace WorkList
{
    using System.Linq;

    using Xamarin.Forms;

    public class ListViewEffect : RoutingEffect
    {
        public static readonly BindableProperty OnProperty = BindableProperty.CreateAttached(
            "On",
            typeof(bool),
            typeof(ListViewEffect),
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
            if (bindable is not ListView listView)
            {
                return;
            }

            if ((bool)newValue)
            {
                listView.Effects.Add(new ListViewEffect());
            }
            else
            {
                var effect = listView.Effects.FirstOrDefault(x => x is ListViewEffect);
                if (effect != null)
                {
                    listView.Effects.Remove(effect);
                }
            }
        }

        public ListViewEffect()
            : base("WorkList.ListViewEffect")
        {
        }
    }
}
