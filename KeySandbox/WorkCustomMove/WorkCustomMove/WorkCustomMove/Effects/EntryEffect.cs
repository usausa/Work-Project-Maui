using System;
namespace WorkCustomMove.Effects
{
    using System.Linq;

    using Xamarin.Forms;

    public sealed class EntryEffect : RoutingEffect
    {
        public static readonly BindableProperty DisableMoveFocusProperty = BindableProperty.CreateAttached(
            "DisableMoveFocus",
            typeof(bool),
            typeof(EntryEffect),
            false,
            propertyChanged: OnDisableMoveFocusChanged);

        public static bool GetDisableMoveFocus(BindableObject view)
        {
            return (bool)view.GetValue(DisableMoveFocusProperty);
        }

        public static void SetDisableMoveFocus(BindableObject view, bool value)
        {
            view.SetValue(DisableMoveFocusProperty, value);
        }

        private static void OnDisableMoveFocusChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is not Element element)
            {
                return;
            }

            if ((bool)newValue)
            {
                element.Effects.Add(new EntryEffect());
            }
            else
            {
                var effect = element.Effects.FirstOrDefault(x => x is EntryEffect);
                if (effect != null)
                {
                    element.Effects.Remove(effect);
                }
            }
        }

        public EntryEffect()
            : base("WorkCustomMove.EntryEffect")
        {
        }
    }
}
