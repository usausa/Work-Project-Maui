namespace KeySample.FormsApp.Behaviors
{
    using System;
    using System.ComponentModel;
    using System.Linq;

    using KeySample.FormsApp.Input;
    using KeySample.FormsApp.Models.Input;

    using Smart.Forms.Interactivity;

    using Xamarin.Forms;

    public sealed class InputBindBehavior : BehaviorBase<Entry>
    {
        private bool updating;

        protected override void OnAttachedTo(Entry bindable)
        {
            base.OnAttachedTo(bindable);

            var controller = InputBind.GetModel(bindable);
            bindable.Completed += BindableOnCompleted;
            bindable.TextChanged += BindableOnTextChanged;
            controller.FocusRequested += ControllerOnFocusRequested;
            controller.PropertyChanged += ControllerOnPropertyChanged;
        }

        protected override void OnDetachingFrom(Entry bindable)
        {
            var controller = InputBind.GetModel(bindable);
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
                var controller = InputBind.GetModel(entry);
                updating = true;
                entry.Text = controller.Text;
                updating = false;
            }
            else if (e.PropertyName == nameof(InputModel.Enable))
            {
                var controller = InputBind.GetModel(entry);
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
            var controller = InputBind.GetModel(entry);
            controller.Text = e.NewTextValue;
        }

        private void BindableOnCompleted(object sender, EventArgs e)
        {
            var entry = (Entry)sender;
            var controller = InputBind.GetModel(entry);
            var ice = new InputCompleteEvent();
            controller.HandleCompleted(ice);
            if (!ice.HasError)
            {
                FocusHelper.MoveFocusInPage(entry, true);
            }
        }
    }

    public sealed class InputBind
    {
        public static readonly BindableProperty ModelProperty = BindableProperty.CreateAttached(
            "Model",
            typeof(IInputController),
            typeof(InputBind),
            null,
            propertyChanged: BindChanged);

        public static IInputController GetModel(BindableObject view)
        {
            return (IInputController)view.GetValue(ModelProperty);
        }

        public static void SetModel(BindableObject view, IInputController value)
        {
            view.SetValue(ModelProperty, value);
        }

        private static void BindChanged(BindableObject bindable, object? oldValue, object? newValue)
        {
            if (bindable is not Entry entry)
            {
                return;
            }

            if (oldValue is not null)
            {
                var behavior = entry.Behaviors.FirstOrDefault(x => x is InputBindBehavior);
                if (behavior is not null)
                {
                    entry.Behaviors.Remove(behavior);
                }
            }

            if (newValue is not null)
            {
                entry.Behaviors.Add(new InputBindBehavior());
            }
        }
    }
}
