using System;
using System.ComponentModel;
using System.Linq;

using Smart.Forms.Interactivity;

using Xamarin.Forms;

namespace WorkEntry
{
    public class InputEntryBehavior : BehaviorBase<Entry>
    {
        public static readonly BindableProperty BindProperty = BindableProperty.CreateAttached(
            "Bind",
            typeof(IInputController),
            typeof(InputEntryBehavior),
            null,
            propertyChanged: BindChanged);

        public static IInputController GetBind(BindableObject view)
        {
            return (IInputController)view.GetValue(BindProperty);
        }

        public static void SetBind(BindableObject view, IInputController value)
        {
            view.SetValue(BindProperty, value);
        }

        private bool updating;

        protected override void OnAttachedTo(Entry bindable)
        {
            base.OnAttachedTo(bindable);

            var controller = GetBind(bindable);
            bindable.Completed += BindableOnCompleted;
            bindable.TextChanged += BindableOnTextChanged;
            controller.FocusRequested += ControllerOnFocusRequested;
            controller.PropertyChanged += ControllerOnPropertyChanged;
        }

        protected override void OnDetachingFrom(Entry bindable)
        {
            var controller = GetBind(bindable);
            bindable.Completed -= BindableOnCompleted;
            bindable.TextChanged -= BindableOnTextChanged;
            controller.FocusRequested -= ControllerOnFocusRequested;
            controller.PropertyChanged -= ControllerOnPropertyChanged;

            base.OnDetachingFrom(bindable);
        }

        private void ControllerOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var entry = AssociatedObject;
            if (entry is null)
            {
                return;
            }

            if (e.PropertyName == nameof(InputModel.Text))
            {
                var controller = GetBind(entry);
                updating = true;
                entry.Text = controller.Text;
                updating = false;
            }
            else if (e.PropertyName == nameof(InputModel.Enable))
            {
                var controller = GetBind(entry);
                entry.IsEnabled = controller.Enable;
            }
        }

        private void ControllerOnFocusRequested(object sender, EventArgs e)
        {
            AssociatedObject?.Focus();
        }

        private void BindableOnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (updating)
            {
                return;
            }

            var entry = (Entry)sender;
            var controller = GetBind(entry);
            controller.Text = e.NewTextValue;
        }

        private void BindableOnCompleted(object sender, EventArgs e)
        {
            var entry = (Entry)sender;
            var controller = GetBind(entry);
            var ice = new InputCompleteEvent();
            controller.HandleCompleted(ice);
            if (!ice.HasError)
            {
                System.Diagnostics.Debug.WriteLine("Move next");
                // TODO Next
            }
        }

        private static void BindChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is not Entry entry)
            {
                return;
            }

            if (oldValue is not null)
            {
                var behavior = entry.Behaviors.FirstOrDefault(x => x is InputEntryBehavior);
                if (behavior is not null)
                {
                    entry.Behaviors.Remove(behavior);
                }
            }

            if (newValue is not null)
            {
                entry.Behaviors.Add(new InputEntryBehavior());
            }
        }
    }
}
