namespace KeyboardSample.Behaviors
{
    using System;

    using KeyboardSample.Input;

    using Smart.Forms.Interactivity;

    using Xamarin.Forms;

    public class EntryControlBehavior : BehaviorBase<Entry>
    {
        public static readonly BindableProperty ControllerProperty =
            BindableProperty.CreateAttached(
                nameof(Controller),
                typeof(IInputController),
                typeof(EntryControlBehavior),
                null,
                propertyChanged: HandleControllerPropertyChanged);

        public IInputController Controller
        {
            get => (IInputController)GetValue(ControllerProperty);
            set => SetValue(ControllerProperty, value);
        }

        protected override void OnAttachedTo(Entry bindable)
        {
            base.OnAttachedTo(bindable);

            bindable.Completed += OnCompleted;
        }

        protected override void OnDetachingFrom(Entry bindable)
        {
            bindable.Completed -= OnCompleted;

            if (Controller != null)
            {
                Controller.FocusRequested -= OnFocusRequested;
            }

            base.OnDetachingFrom(bindable);
        }

        private static void HandleControllerPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            ((EntryControlBehavior)bindable).OnControllerPropertyChanged(oldValue as IInputController, newValue as IInputController);
        }

        private void OnControllerPropertyChanged(IInputController oldValue, IInputController newValue)
        {
            if (oldValue == newValue)
            {
                return;
            }

            if (oldValue != null)
            {
                oldValue.FocusRequested -= OnFocusRequested;
            }

            if (newValue != null)
            {
                newValue.FocusRequested += OnFocusRequested;
            }
        }

        private void OnFocusRequested(object sender, EventArgs e)
        {
            AssociatedObject.Focus();
        }

        private void OnCompleted(object sender, EventArgs e)
        {
            Controller.OnCompleted(AssociatedObject.Text);
        }
    }
}
